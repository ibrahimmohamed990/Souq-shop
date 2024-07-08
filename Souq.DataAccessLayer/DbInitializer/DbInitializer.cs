using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Utilities;


namespace Souq.DataAccessLayer.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IdentityUserDbContext _dbIdentityContext;

        public DbInitializer(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext dbContext, IdentityUserDbContext dbIdentityContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _dbIdentityContext = dbIdentityContext;
        }
        public async Task Initialize()
        {
            try
            {
                if(_dbContext.Database.GetPendingMigrations().Count() > 0)
                    _dbContext.Database.Migrate();

                if (_dbIdentityContext.Database.GetPendingMigrations().Count() > 0)
                    _dbIdentityContext.Database.Migrate();
            }
            catch(Exception ex)
            {
                throw;
            }

            if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
            }

            var adminUser = new ApplicationUser
            {
                Email = "admin@souq.com",
                UserName = "admin@souq.com",
                Name = "Administrator",
                Address = "Tanta",
                City = "Tanta",
                PhoneNumber = "0123456789",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };

            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await _userManager.CreateAsync(adminUser, "#Pass123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, SD.AdminRole);
                }
            }
        }
    }
}
