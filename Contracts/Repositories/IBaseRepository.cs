using System.Linq.Expressions;

namespace portal_agile.Contracts.Repositories
{
    public interface IBaseRepository<T, TKey> where T : class
    {
        /// <summary>
        /// Get all entities of type T
        /// </summary>
        /// <returns>Type of <typeparamref name="T"/></returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Type of <typeparamref name="T"/></returns>
        Task<T?> GetById(TKey id);

        /// <summary>
        /// Search for records of type T
        /// </summary>
        /// <param name="query"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Search(IQueryable<T> query, string keyword);

        /// <summary>
        /// Create record of type T
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Create(T entity);

        /// <summary>
        /// Updates a record of type T
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Updates a specific property of an entity identified by its key
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        /// <param name="propertyName">Column name or property to update</param>
        /// <param name="propertyValue">New value for the property</param>
        /// <returns>The updated entity</returns>
        Task<T> UpdateFromKey(TKey id, string propertyName, object propertyValue);

        /// <summary>
        /// Deletes a record of type T
        /// </summary>
        /// <param name="entity"></param>
        Task<T> SoftDelete(T entity);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
