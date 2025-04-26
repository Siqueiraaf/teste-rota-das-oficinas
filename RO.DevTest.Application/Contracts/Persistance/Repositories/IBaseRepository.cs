using System.Linq.Expressions;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories;

public interface IBaseRepository<T> where T : class {

    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="entity"> The entity to be create </param>
    /// <param name="cancellationToken"> Cancellation token </param>
    /// <returns> The created entity </returns>
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the first entity that matches with the <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate">
    /// The <see cref="Expression"/> to be used while
    /// looking for the entity
    /// </param>
    /// <returns>
    /// The <typeparamref name="T"/> entity, if found. Null otherwise. </returns>
    T? Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Updates an entity entry on the database
    /// </summary>
    /// <param name="entity"> The entity to be added </param>
    void Update(T entity);

    /// <summary>
    /// Deletes one entry from the database
    /// </summary>
    /// <param name="entity"> The entity to be deleted </param>
    void Delete(T entity);
    
    // ***add my solutions***
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <returns>List of all entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Gets entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">Filter condition</param>
    /// <returns>List of matching entities</returns>
    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>> expression, 
        Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including related entities
    /// </summary>
    /// <param name="predicate">Optional filter condition</param>
    /// <param name="orderBy">Optional ordering function</param>
    /// <param name="includeString">Optional related entity to include</param>
    /// <param name="disableTracking">Whether to disable change tracking</param>
    /// <returns>List of matching entities</returns>
    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets entities with advanced filtering, ordering, and including multiple related entities
    /// </summary>
    /// <param name="predicate">Optional filter condition</param>
    /// <param name="orderBy">Optional ordering function</param>
    /// <param name="includes">Optional list of related entities to include</param>
    /// <param name="disableTracking">Whether to disable change tracking</param>
    /// <returns>List of matching entities</returns>
    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = true);

    /// <summary>
    /// Counts entities that match the specified predicate
    /// </summary>
    /// <param name="predicate">Optional filter condition</param>
    /// <returns>Count of matching entities</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    Task DeleteAsync(T entity);
}
