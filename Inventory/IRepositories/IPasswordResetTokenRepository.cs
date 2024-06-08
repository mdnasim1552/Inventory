using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.IRepositories
{
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
    }
}
