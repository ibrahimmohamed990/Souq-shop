using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Souq.Entities.Models;
using Souq.Utilities;
using System.Security.Claims;

namespace Souq.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;

            var users = _userManager.Users.Where(x => x.Id != userId).ToList();

            return View(users);
        }
        public async Task<IActionResult> LockUnlockAsync(string? id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            if (user is null)
                return NotFound();
            var time = DateTime.Now;
            if (user.LockoutEnd is null || user.LockoutEnd < DateTime.Now)
                time = DateTime.MaxValue;

            user.LockoutEnd = time.ToUniversalTime();

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Users", new {area = "Admin"});
        }
    }
}
