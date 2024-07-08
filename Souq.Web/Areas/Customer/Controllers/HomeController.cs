using Humanizer.Localisation.TimeToClockNotation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Entities.ViewModels;
using Souq.Utilities;
using Stripe;
using Stripe.Climate;
using System.Security.Claims;
using X.PagedList;
using Product = Souq.Entities.Models.Product;

namespace Souq.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int? page)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll();
            int pageSize = 6;
            page = page ?? 0;
            var pageNumber = (page == 0) ? 1 : page;
            var onePageOfCategories = categories.ToPagedList((int)pageNumber, pageSize);

            return View(onePageOfCategories);
        }

        [HttpGet("/Customer/Home/Details/{ProductId}")]
        public async Task<IActionResult> Details(int ProductId)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefault(x => x.Id == ProductId, Include:"Category");
            if (product == null)
                return NotFound();
            var products = await _unitOfWork.ProductRepository.GetAll(x => x.Category.Name == product.Category.Name, Include:"Category");
           
            List<Product> FourProducts = new List<Product>();
            int cnt = 4;
            foreach(var p in products)
            {
                if(cnt > 0 && product.Id != p.Id)
                {
                    FourProducts.Add(p);
                    --cnt;
                }
            }
            ViewBag.RelatedProducts = FourProducts;
            ViewBag.Referer = Request.Headers["Referer"].ToString();

            var IsProductExsit = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.ProductId == ProductId);
            int count = 1;
            if(IsProductExsit is not null)
                count = IsProductExsit.Count;

            ShoppingCart productShop = new ShoppingCart
            {
                ProductId = ProductId,
                Product = product,
                Count = count
            };

            return View(productShop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart, int CatId, string Referer)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.UserId = claim.Value;

            var oldCart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.UserId == claim.Value && x.ProductId == cart.ProductId, tracking: false).Result;
            
            if(oldCart is null)
                await _unitOfWork.ShoppingCartRepository.Add(cart);
            else
            {
                cart.ShoppingCartId = oldCart.ShoppingCartId;
                _unitOfWork.ShoppingCartRepository.UpdateCart(cart);
            }

            await _unitOfWork.Complete();

            return Redirect(Referer);
            //return RedirectToAction("Products", new {id = CatId});
        }
        public async Task<IActionResult> Products(int? catId, int? page, string? productName)
        {
            //ViewBag.Referer = Request.Headers["Referer"].ToString();

            IEnumerable<Product> products;
            if (catId is null && productName is null)
                products = await _unitOfWork.ProductRepository.GetAll();
            else if (catId is null && productName is not null)
                products = _unitOfWork.ProductRepository.Search(productName);
            else if (catId is not null && productName is not null)
                products = _unitOfWork.ProductRepository.Search((int)catId, productName);
            else if(catId is not null && productName is null)
                products = await _unitOfWork.ProductRepository.GetAll(x => x.CategoryId == catId);
            else
                products = await _unitOfWork.ProductRepository.GetAll();


            int pageSize = 8;
            page = page ?? 0;
            var pageNumber = (page == 0) ? 1 : page;
            var onePageOfProducts = products.ToPagedList((int)pageNumber, pageSize);
            
            if(catId is not null)  ViewBag.CategoryId = catId;
            if(productName is not null)  ViewBag.ProductName = productName;

            return View(onePageOfProducts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int Count)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartItem = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                x => x.UserId == userId && x.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    UserId = userId,
                    ProductId = productId,
                    Count = Count
                };
                await _unitOfWork.ShoppingCartRepository.Add(cartItem);
            }
            else
            {
                cartItem.Count += Count;
                _unitOfWork.ShoppingCartRepository.Update(cartItem);
            }

            await _unitOfWork.Complete();

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Orders(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var orders = await _unitOfWork.OrderHeaderRepository.GetAll(o => o.UserId == userId);

            // Retrieve detailed order information (products, images, quantities)
            var orderDetails = new List<OrderDetailsViewModel>();
            foreach (var order in orders)
            {
                var products = await _unitOfWork.OrderDetailsRepository
                    .GetAll(od => od.OrderId == order.Id, Include: "Product");

                var orderDetailViewModel = new OrderDetailsViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalPrice,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    TrackingNumber = order.TrackingNumber,
                    ShippingDate = order.ShippingDate,
                    Address = order.Address,
                    City = order.City,
                    Products = products.Select(od => new ProductViewModel
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.Name,
                        ProductImage = od.Product.ImageUrl, // Assuming ImageUrl is a property of Product
                        Quantity = od.Count
                    }).ToList()
                };

                orderDetails.Add(orderDetailViewModel);
            }
            orderDetails = orderDetails.OrderByDescending(x => x.OrderDate).ToList();

            int pageSize = 6;
            var pageNumber = page ?? 1;
            var onePageOfOrders = orderDetails.ToPagedList(pageNumber, pageSize);

            return View(onePageOfOrders);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderId);

            if (orderHeader.OrderStatus == SD.Approve || orderHeader.OrderStatus == SD.Proccessing)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId,
                    Amount = (long)orderHeader.TotalPrice * 100
                };

                var refundService = new RefundService();
                var refund = refundService.Create(option);
                _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderHeader.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderHeader.Id, SD.Cancelled, SD.Cancelled);
            }
            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return NotFound();

            TempData["Update"] = "Order has been Cancelled Successfully.";
            return RedirectToAction(nameof(Orders));
        }
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
