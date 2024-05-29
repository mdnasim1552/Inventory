using Inventory.Repositories;

namespace Inventory.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        ICredentialRepository Credential { get; }
        void Saved();
        Task<bool> SaveAsync();
    }
}
