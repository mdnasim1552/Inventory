using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class PasswordResetTokenRepository:Repository<PasswordResetToken>,IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _db;
        public PasswordResetTokenRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
    }
}
