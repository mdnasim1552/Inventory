using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class ProductStoreRepository : Repository<ProductStore>, IProductStoreRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductStoreRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
