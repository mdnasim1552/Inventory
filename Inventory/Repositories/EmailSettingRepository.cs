using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Models;
using Inventory.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace Inventory.Repositories
{
    public class EmailSettingRepository : Repository<EmailSetting>, IEmailSettingRepository
    {
        private readonly ApplicationDbContext _db;
        public EmailSettingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<bool> SendEmailAsync(string adminEmail,string email, string subject, string message)
        {
            try
            {
                //var adminEmail = await (from c in _db.Credentials
                //                        join r in _db.Userroles
                //                        on c.RoleId equals r.RoleId
                //                        where r.Role == "Admin"
                //                        select c.Email).SingleOrDefaultAsync();
                var emailsettings = await (from e in _db.EmailSettings where e.From == adminEmail select e).SingleOrDefaultAsync();
                MailMessage mailmessage = new MailMessage()
                {
                    From = new MailAddress(emailsettings.From),
                    Subject = subject,
                    Body = message,
                    BodyEncoding=System.Text.Encoding.ASCII,
                    IsBodyHtml= true
                };
                mailmessage.To.Add(email);
                SmtpClient smtp = new SmtpClient(emailsettings.SmtpServer)
                {
                    Port = emailsettings.Port,
                    Credentials = new NetworkCredential(emailsettings.From, emailsettings.SecretKey),
                    EnableSsl = emailsettings.EnableSsl
                };
                await smtp.SendMailAsync(mailmessage);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
