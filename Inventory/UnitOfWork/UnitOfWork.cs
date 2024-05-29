using Inventory.Data;
using Inventory.Repositories;

namespace Inventory.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICredentialRepository Credential { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            //Comp=new CompinfsRepository(context);
            Credential = new CredentialRepository(context);
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
