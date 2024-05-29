using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Inventory.Data;
using Inventory.Models;
using Inventory.UnitOfWork;

namespace Inventory.Repositories
{
    public class CredentialRepository:Repository<Credential>, ICredentialRepository
    {
        private readonly ApplicationDbContext _db;
        public CredentialRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        //public void Update(Credential credential)
        //{
        //    _db.Credentials.Update(credential);
        //}
        //public async Task<bool> EmailExistsAsync(string email)
        //{
        //    return await _db.Credentials.AnyAsync(c => c.Email == email);
        //}
        
    }
}
