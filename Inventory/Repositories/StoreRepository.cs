using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class StoreRepository:Repository<Store>, IStoreRepository
    {
        private readonly ApplicationDbContext _db;
        public StoreRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
