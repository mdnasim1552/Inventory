using OnlineCV.Models;
using OnlineCV.UnitOfWork;

namespace OnlineCV.Repositories
{
    public interface ICredentialRepository:IRepository<Credential>
    {
        void Update(Credential credential);
        Task<bool> EmailExistsAsync(string email);
    }
}
