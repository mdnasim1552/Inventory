using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.IRepositories
{
    public interface IEmailSettingRepository : IRepository<EmailSetting>
    {
        Task<bool> SendEmailAsync(string adminEmail, string email, string subject, string message);
    }
}
