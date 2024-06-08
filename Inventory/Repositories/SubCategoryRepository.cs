using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class SubCategoryRepository: Repository<Category>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
