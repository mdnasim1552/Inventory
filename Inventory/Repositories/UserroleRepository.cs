using Inventory.Data;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class UserroleRepository:Repository<Userrole>, IUserroleRepository
    {
        private readonly ApplicationDbContext _db;
        public UserroleRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
