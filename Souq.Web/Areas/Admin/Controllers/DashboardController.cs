using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Repositories;
using Souq.Entities.Repositories.Identity;
using Souq.Utilities;

namespace Souq.Web.Areas.Admin.Controllers
{
    [Area(SD.AdminRole)]
    [Authorize(Roles = SD.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkForIdentity _unitOfWorkForIdentity;

        public DashboardController(IUnitOfWork unitOfWork, IUnitOfWorkForIdentity unitOfWorkForIdentity)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkForIdentity = unitOfWorkForIdentity;
        }
        public async Task<IActionResult> Index()
        {
            var ordersCount = await _unitOfWork.OrderHeaderRepository.GetAll();
            ViewBag.Orders = ordersCount.Count();
            var approvedOrdersCount = await _unitOfWork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Approve);
            ViewBag.ApprovedOrders = approvedOrdersCount.Count();
            var users = await _unitOfWorkForIdentity.ApplicationUserRepository.GetAll();
            ViewBag.Users = users.Count();
            var products = await _unitOfWork.ProductRepository.GetAll();
            ViewBag.Products = products.Count();

            return View();
        }
    }
}
