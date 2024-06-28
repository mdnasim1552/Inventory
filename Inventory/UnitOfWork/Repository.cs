using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using System.Linq.Expressions;

namespace Inventory.UnitOfWork
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<TEntity> dbSet;
        public Repository(ApplicationDbContext db)
        {
            this._db = db;
            this.dbSet=_db.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public TEntity Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<TEntity> GetAsync(int? id)
        {
            return await dbSet.FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
        }
        public void Update(TEntity entity)
        {
            dbSet.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }
        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.SingleOrDefaultAsync(predicate);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
    }
}
