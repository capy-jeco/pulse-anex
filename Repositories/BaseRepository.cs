using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;

namespace portal_agile.Repositories
{
    public class BaseRepository<T, TKey, TContext> : IBaseRepository<T, TKey>
        where T : class
        where TKey: notnull
        where TContext: DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(
            TContext context)
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

        public async Task<T> UpdateFromKey(TKey id, string propertyName, object propertyValue)
        {
            // Find the entity
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"{nameof(T)} with ID {id} not found");
            }

            // Get property info
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' does not exist on type {typeof(T).Name}");
            }

            // Get old value for auditing
            var oldValue = propertyInfo.GetValue(entity);

            // Convert the value to the correct type
            object? typedValue = ConvertValue(propertyValue, propertyInfo.PropertyType);

            // Set the value
            propertyInfo.SetValue(entity, typedValue);

            // Mark only this property as modified
            _context.Entry(entity).Property(propertyName).IsModified = true;

            // Save changes
            await _context.SaveChangesAsync();

            return entity;
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

        private object? ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType.IsEnum && value is string stringValue)
            {
                return Enum.Parse(targetType, stringValue);
            }

            if (value is JsonElement jsonElement)
            {
                // Handle values coming from System.Text.Json
                return ConvertFromJsonElement(jsonElement, targetType);
            }

            return Convert.ChangeType(value, targetType);
        }

        private object? ConvertFromJsonElement(JsonElement element, Type targetType)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (targetType == typeof(int)) return element.GetInt32();
                    if (targetType == typeof(long)) return element.GetInt64();
                    if (targetType == typeof(decimal)) return element.GetDecimal();
                    if (targetType == typeof(double)) return element.GetDouble();
                    return element.GetDecimal();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                default:
                    return element.GetRawText();
            }
        }
    }
}
