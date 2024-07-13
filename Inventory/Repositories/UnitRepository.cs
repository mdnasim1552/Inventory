using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class UnitRepository:Repository<Unit>,IUnitRepository
    {
        private readonly ApplicationDbContext _db;
        public UnitRepository(ApplicationDbContext db):base(db)
        {
            _db= db;
        }
    }
}
