using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Inventory.Data;
using Inventory.Models;
using Inventory.UnitOfWork;
using Inventory.IRepositories;

namespace Inventory.Repositories
{
    public class CredentialRepository:Repository<Credential>, ICredentialRepository
    {
        private readonly ApplicationDbContext _db;
        public CredentialRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<List<int>> GetUserIdListOnParent(int parentId)
        {
            var userIdList = await (from c in _db.Credentials where c.ParentId == parentId select c.Id).ToListAsync<int>();
            //var adminEmail = await (from c in _db.Credentials
            //                        join r in _db.Userroles
            //                        on c.RoleId equals r.RoleId
            //                        where r.Role == "Admin"
            //                        select c.Email).SingleOrDefaultAsync();
            return userIdList;
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
