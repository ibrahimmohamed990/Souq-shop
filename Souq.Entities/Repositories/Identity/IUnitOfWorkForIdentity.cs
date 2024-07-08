using Souq.Entities.Models;

namespace Souq.Entities.Repositories.Identity
{
    public interface IUnitOfWorkForIdentity : IDisposable
    {
        IApplicationUserRepository ApplicationUserRepository { get; }
        Task<int> Complete();
    }
}
