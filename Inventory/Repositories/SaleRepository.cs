using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class SaleRepository:Repository<Sale>, ISaleRepository
    {
        private readonly ApplicationDbContext _db;
        public SaleRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
