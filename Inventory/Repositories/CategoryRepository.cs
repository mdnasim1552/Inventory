using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class CategoryRepository:Repository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
