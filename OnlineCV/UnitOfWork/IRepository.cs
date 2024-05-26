using System.Linq.Expressions;

namespace OnlineCV.UnitOfWork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(int id); // Asynchronous method to get an entity by id
        Task<IEnumerable<TEntity>> GetAllAsync(); // Asynchronous method to get all entities
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate); // Asynchronous method to find entities by predicate
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
    }
}
