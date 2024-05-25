using OnlineCV.Repositories;

namespace OnlineCV.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        ICredentialRepository Credential { get; }
        void Saved();
        Task<bool> SaveAsync();
    }
}
