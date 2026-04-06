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
        public IBrandRepository Brand { get; private set; }
        public ISubCategoryRepository SubCategory { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICustomerRepository Customer { get; private set; }
        public ISupplierRepository Supplier { get; set; }
        public IStoreRepository Store { get; private set; }
        public IPurchaseRepository Purchase { get; set; }
        public IUnitRepository Unit { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            //Comp=new CompinfsRepository(context);
            Credential = new CredentialRepository(context);
            Userrole = new UserroleRepository(context);
            EmailSetting = new EmailSettingRepository(context);
            PasswordResetToken=new PasswordResetTokenRepository(context);
            Brand = new BrandRepository(context);
            SubCategory = new SubCategoryRepository(context);
            Category = new CategoryRepository(context);
            Product=new ProductRepository(context);
            Customer=new CustomerRepository(context);
            Supplier=new SupplierRepository(context);
            Store=new StoreRepository(context);
            Purchase=new PurchaseRepository(context);
            Unit = new UnitRepository(context);
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
