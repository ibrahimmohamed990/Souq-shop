using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Entities.Repositories.Identity;
using Souq.Entities.ViewModels;
using Souq.Utilities;
using Stripe;
using Stripe.Checkout;
using System.Configuration;
using System.Security.Claims;

namespace Souq.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkForIdentity _unitOfWorkForIdentity;
        private readonly IConfiguration _configuration;

        public CartController(IUnitOfWork unitOfWork, IUnitOfWorkForIdentity unitOfWorkForIdentity, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkForIdentity = unitOfWorkForIdentity;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var carts = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, Include: "Product");

            var ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = carts
            };

            foreach (var item in ShoppingCartVM.CartsList)
                ShoppingCartVM.TotalPriceForCarts += item.Count * item.Product.Price;

            return View(ShoppingCartVM);
        }

        public async Task<IActionResult> Plus(int cartid)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.ShoppingCartId == cartid);
            if (cart is null)
                return NotFound();
            cart.Count += 1;
            await _unitOfWork.Complete();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Minus(int cartid)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.ShoppingCartId == cartid);
            if (cart is null)
                return NotFound();

            if (cart.Count > 1)
            {
                cart.Count -= 1;
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
            }

            await _unitOfWork.Complete();

            var carts = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == cart.UserId);
            if (!carts.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int cartid)
        {
            var cart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.ShoppingCartId == cartid);
            if (cart is null)
                return NotFound();

            _unitOfWork.ShoppingCartRepository.Remove(cart);
            await _unitOfWork.Complete();

            var carts = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == cart.UserId);

            if (!carts.Any())
                return RedirectToAction("Index", "Home");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var carts = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, Include: "Product");

            var shoppingCartVM = new ShoppingCartVM()
            {
                CartsList = carts,
                OrderHeader = new OrderHeader()
            };

            var user = await _unitOfWorkForIdentity.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == userId);

            shoppingCartVM.OrderHeader.UserId = user.Id;
            shoppingCartVM.OrderHeader.UserName = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.Address = user.Address;
            shoppingCartVM.OrderHeader.City = user.City;

            foreach (var item in shoppingCartVM.CartsList)
                shoppingCartVM.TotalPriceForCarts += item.Count * item.Product.Price;

            shoppingCartVM.OrderHeader.TotalPrice = shoppingCartVM.TotalPriceForCarts;

            if (shoppingCartVM.CartsList.Count() <= 0)
                return RedirectToAction("Index");
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            shoppingCartVM.CartsList = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, Include: "Product");

            // Check if the order already exists
            var existingOrder = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault (x => x.UserId == userId && x.OrderStatus == SD.Pending && x.TotalPrice == shoppingCartVM.TotalPriceForCarts);
            //var existingOrder = _unitOfWork.OrderHeaderRepository.GetLastOrder(userId);
            
            //  && shoppingCartVM.TotalPriceForCarts == existingOrder.TotalPrice

            // Create a new order if it doesn't exist
            shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.UserId = userId;
            shoppingCartVM.OrderHeader.TotalPrice = shoppingCartVM.TotalPriceForCarts;

            if (existingOrder != null && existingOrder.SessionId != null && existingOrder.TotalPrice == shoppingCartVM.OrderHeader.TotalPrice)
            {
                // Redirect to payment for the existing order
                ViewBag.OrderId = existingOrder.Id;
                return View("_PayExistingOrder");
            }
            await _unitOfWork.OrderHeaderRepository.Add(shoppingCartVM.OrderHeader);
            await _unitOfWork.Complete();

            foreach (var item in shoppingCartVM.CartsList)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Product = item.Product,
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                };
                await _unitOfWork.OrderDetailsRepository.Add(orderDetail);
                await _unitOfWork.Complete();
            }

            var domain = _configuration["BaseUrl"];

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}customer/cart/orderconfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = $"{domain}Customer/Cart/index"
            };

            foreach (var item in shoppingCartVM.CartsList)
            {
                var sessionLineOption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Product.Price * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineOption);
            }

            var service = new SessionService();
            Session session;
            try
            {
                session = service.Create(options);
            }
            catch (StripeException ex)
            {
                Console.WriteLine("StripeException: " + ex.Message);
                throw;
            }

            shoppingCartVM.OrderHeader.SessionId = session.Id;
            shoppingCartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;

            _unitOfWork.OrderHeaderRepository.Update(shoppingCartVM.OrderHeader);
            await _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayExistingOrder(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderId);
            var orderDetails = await _unitOfWork.OrderDetailsRepository.GetAll(x => x.OrderId == orderId, Include: "Product");

            if (orderHeader == null || orderHeader.OrderStatus != SD.Pending || orderHeader.PaymentStatus != SD.Pending)
            {
                return NotFound();
            }

            // Create Stripe session for the existing order
            var domain = _configuration["BaseUrl"];

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}customer/cart/orderconfirmation?id={orderId}",
                CancelUrl = $"{domain}customer/cart/index"
            };

            foreach (var item in orderDetails)
            {
                var sessionLineOption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Product.Price * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineOption);
            }

            var service = new SessionService();
            Session session;
            try
            {
                session = service.Create(options);
            }
            catch (StripeException ex)
            {
                // Log the exception to get more details about the error
                Console.WriteLine("StripeException: " + ex.Message);
                throw;
            }

            orderHeader.SessionId = session.Id;
            orderHeader.PaymentIntentId = session.PaymentIntentId;

            _unitOfWork.OrderHeaderRepository.Update(orderHeader);
            await _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                orderHeader.PaymentDate = DateTime.Now;
                await _unitOfWork.Complete();
            }

            var shoppingCarts = await _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == orderHeader.UserId);
            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
            await _unitOfWork.Complete();
            return View(id);
        }
    }
}
