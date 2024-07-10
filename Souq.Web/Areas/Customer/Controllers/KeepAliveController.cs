using Microsoft.AspNetCore.Mvc;

namespace Souq.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class KeepAliveController : Controller
    {
        [Route("/customer/keep-alive")]
        [HttpGet]
        public IActionResult KeepAlive()
        {
            return Ok();
        }
    }
}
