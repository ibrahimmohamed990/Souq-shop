using Microsoft.AspNetCore.Identity;
using Souq.DataAccessLayer.Context;
using Souq.Entities.Repositories;
using Souq.Entities.Repositories.Identity;
using System.Collections;

namespace Souq.DataAccessLayer.Implementation.Identity
{
    public class UnitOfWorkForIdentity : IUnitOfWorkForIdentity
    {
        private readonly IdentityUserDbContext _context;

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }

        private Hashtable? repositories;

        public UnitOfWorkForIdentity(IdentityUserDbContext context)
        {
            _context = context;
            ApplicationUserRepository = new ApplicationUserRepository(context);
        }
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
