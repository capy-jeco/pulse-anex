using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace portal_agile.Repositories
{
    public class BaseRepository<T, TKey, TContext> : IBaseRepository<T, TKey>
        where T : class
        where TKey: notnull
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
        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public virtual async Task<T?> GetById(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> Search(IQueryable<T> query, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await query.ToListAsync();

            var parameter = Expression.Parameter(typeof(T), "x");
            var keywordPattern = Expression.Constant($"%{keyword}%", typeof(string));
            var efFunctions = Expression.Constant(EF.Functions);

            Expression? combinedExpression = null;

            foreach (var prop in typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string)))
            {
                // x.Prop
                var propertyAccess = Expression.Property(parameter, prop);

                // EF.Functions.Like(x.Prop, "%keyword%")
                var likeCall = Expression.Call(
                    typeof(DbFunctionsExtensions),
                    nameof(DbFunctionsExtensions.Like),
                    Type.EmptyTypes,
                    efFunctions,
                    propertyAccess,
                    keywordPattern
                );

                combinedExpression = combinedExpression == null
                    ? (Expression)likeCall
                    : Expression.OrElse(combinedExpression, likeCall);
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public virtual async Task<T> Create(T entity)
        {
            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public virtual Task<T> SoftDelete(T entity)
        {
            // Set soft delete flag — customize this based on your model
            typeof(T).GetProperty("IsDeleted")?.SetValue(entity, true);

            _dbSet.Update(entity);

            return Task.FromResult(entity);
        }

        /// <inheritdoc/>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
