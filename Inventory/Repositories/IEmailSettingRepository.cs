using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public interface IEmailSettingRepository: IRepository<EmailSetting>
    {
        Task<bool> SendEmailAsync(string email,string subject,string message);
    }
}
