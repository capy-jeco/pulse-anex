using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;

namespace portal_agile.Repositories
{
    public class BaseRepository<T, TContext> : IBaseRepository<T> 
        where T : class
        where TContext: DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<T?> GetById(T id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<T> Create(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
