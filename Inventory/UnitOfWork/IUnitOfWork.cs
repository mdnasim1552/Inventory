using Inventory.Repositories;

namespace Inventory.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        ICredentialRepository Credential { get; }
        IUserroleRepository Userrole { get; }
        IEmailSettingRepository EmailSetting { get; }
        void Saved();
        Task<bool> SaveAsync();
    }
}
