using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Interfaces;
using System.Linq.Expressions;

namespace OrderManagementSystem.Data
{
    /// <summary>
    /// Generic repository implementation for data access operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly OrderManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(OrderManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>IQueryable of entities</returns>
        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Get entities by predicate
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <returns>IQueryable of filtered entities</returns>
        public virtual IQueryable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        /// <summary>
        /// Get entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity or null</returns>
        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Update existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Check if entity exists
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <returns>True if exists, false otherwise</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Get count of entities
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <returns>Count of entities</returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Get all entities with includes
        /// </summary>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>IQueryable with includes</returns>
        public virtual IQueryable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }

        /// <summary>
        /// Get entities by predicate with includes
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <param name="includes">Navigation properties to include</param>
        /// <returns>IQueryable with includes</returns>
        public virtual IQueryable<T> GetByWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.Where(predicate);
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }
    }
} 