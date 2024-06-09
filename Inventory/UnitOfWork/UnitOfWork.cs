using Inventory.Data;
using Inventory.IRepositories;
using Inventory.Repositories;

namespace Inventory.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICredentialRepository Credential { get; private set; }
        public IUserroleRepository Userrole { get; private set; }
        public IEmailSettingRepository EmailSetting { get; private set; }
        public IPasswordResetTokenRepository PasswordResetToken { get; private set; }
        public IBrandRepository BrandRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            //Comp=new CompinfsRepository(context);
            Credential = new CredentialRepository(context);
            Userrole = new UserroleRepository(context);
            EmailSetting = new EmailSettingRepository(context);
            PasswordResetToken=new PasswordResetTokenRepository(context);
            BrandRepository = new BrandRepository(context);
        }

       

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Saved()
        {
            _context.SaveChanges();
        }
        public async Task<bool> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle exceptions as needed
                return false;
            }
        }

    }
}
