using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class SaleItemRepository : Repository<SaleItem>, ISaleItemRepository
    {
        private readonly ApplicationDbContext _db;
        public SaleItemRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
