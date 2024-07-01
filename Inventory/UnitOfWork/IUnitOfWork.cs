using Inventory.IRepositories;

namespace Inventory.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        ICredentialRepository Credential { get; }
        IUserroleRepository Userrole { get; }
        IEmailSettingRepository EmailSetting { get; }
        IPasswordResetTokenRepository PasswordResetToken { get; }
        IBrandRepository Brand { get; }
        ISubCategoryRepository SubCategory { get; }
        ICategoryRepository Category { get; }
        void Saved();
        Task<bool> SaveAsync();
    }
}
