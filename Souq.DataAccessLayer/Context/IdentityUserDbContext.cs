using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Souq.Entities.Models;

namespace Souq.DataAccessLayer.Context
{
    public class IdentityUserDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> options): base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
