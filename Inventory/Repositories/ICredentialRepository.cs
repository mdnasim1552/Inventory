using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public interface ICredentialRepository:IRepository<Credential>
    {
        //void Update(Credential credential);
        //Task<bool> EmailExistsAsync(string email);     
    }
}
