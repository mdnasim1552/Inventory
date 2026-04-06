using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class PurchaseRepository:Repository<Purchase>, IPurchaseRepository
    {
        private readonly ApplicationDbContext _db;
        public PurchaseRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
