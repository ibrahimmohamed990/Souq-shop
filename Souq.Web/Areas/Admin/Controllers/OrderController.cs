using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Models;
using Souq.Entities.Repositories;
using Souq.Entities.Repositories.Identity;
using Souq.Entities.ViewModels;
using Souq.Utilities;
using Stripe;

namespace Souq.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkForIdentity _unitOfWorkForIdentity;

        public OrderController(IUnitOfWork unitOfWork, IUnitOfWorkForIdentity unitOfWorkForIdentity)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkForIdentity = unitOfWorkForIdentity;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var orderHeaders = await _unitOfWork.OrderHeaderRepository.GetAll();

            List<OrderHeaderVM> orderHeadersVM = new List<OrderHeaderVM>();

            foreach (var item in orderHeaders)
            {
                var user = await _unitOfWorkForIdentity.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == item.UserId);
                orderHeadersVM.Add(new OrderHeaderVM
                {
                    OrderHeader = item,
                    ApplicationUser = user
                });
            }
            return Json(new { data = orderHeadersVM });
        }

        public async Task<IActionResult> Details(int orderid)
        {
            //orderid = Convert.ToInt32(Request.Query["orderid"]);
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == orderid);
            var user = await _unitOfWorkForIdentity.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == orderHeader.UserId);

            OrderHeaderVM orderHeaderVM = new OrderHeaderVM
            {
                OrderHeader = orderHeader,
                ApplicationUser = user
            };

            var orderDetails = await _unitOfWork.OrderDetailsRepository.GetAll(x => x.OrderId == orderid, Include:"Product");

            OrderVM orderVM = new OrderVM
            {
                OrderHeader = orderHeaderVM,
                OrderDetails = orderDetails
            };
            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderDetails(OrderVM orderVM, int id)
        {
            if(!ModelState.IsValid)
                return View("Details",orderVM);

            _unitOfWork.OrderHeaderRepository.Update(orderVM.OrderHeader.OrderHeader);
            var result = await _unitOfWork.Complete();
            if(result <= 0)
                return NotFound();
            TempData["Update"] = "Order has updated successfully.";
            return RedirectToAction("Details", new {orderid = id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProccess(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderHeader.Id, SD.Proccessing, orderHeader.PaymentStatus);
            var result = await _unitOfWork.Complete();
            if(result <= 0)
                return NotFound();


            TempData["Update"] = "Order status has updated successfully.";
            return RedirectToAction("Details", new { orderid = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartShipping(OrderVM orderVM, int id)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);
            _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderHeader.Id, SD.Shipped,orderHeader.PaymentStatus);
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeaderRepository.Update(orderHeader);
            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return NotFound();

            TempData["Update"] = "Order has shipped successfully.";
            return RedirectToAction("Details", new { orderid = id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id);
            
            if(orderHeader.OrderStatus == SD.Approve || orderHeader.OrderStatus == SD.Proccessing)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId,
                    Amount = (long)orderHeader.TotalPrice*100
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
            return RedirectToAction("Details", new { orderid = id });
        }
    }
}
