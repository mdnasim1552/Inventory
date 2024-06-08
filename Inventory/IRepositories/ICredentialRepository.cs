using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.IRepositories
{
    public interface ICredentialRepository : IRepository<Credential>
    {
        //void Update(Credential credential);
        //Task<bool> EmailExistsAsync(string email);     
    }
}
