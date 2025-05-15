namespace portal_agile.Contracts.Repositories
{
    public interface IBaseRepository<T> where T : class
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
        Task<T?> GetById(T id);

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
        /// Deletes a record of type T
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
