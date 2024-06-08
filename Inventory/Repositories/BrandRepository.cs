using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;
using NuGet.Protocol.Core.Types;

namespace Inventory.Repositories
{
    public class BrandRepository:Repository<Brand>,IBrandRepository
    {
        private readonly ApplicationDbContext _db;
        public BrandRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
