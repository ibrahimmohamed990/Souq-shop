using Souq.DataAccessLayer.Context;
using Souq.Entities.Models;
using Souq.Entities.Repositories.Identity;


namespace Souq.DataAccessLayer.Implementation.Identity
{
    public class ApplicationUserRepository : GenericRepositoryForIdentity<ApplicationUser>, IApplicationUserRepository
    {
        private readonly IdentityUserDbContext _context;

        public ApplicationUserRepository(IdentityUserDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
