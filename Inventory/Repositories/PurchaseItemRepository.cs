using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class PurchaseItemRepository : Repository<PurchaseItem>, IPurchaseItemRepository
    {
        private readonly ApplicationDbContext _db;
        public PurchaseItemRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
