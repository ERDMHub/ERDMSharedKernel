using ERDM.Shared.Kernel.Entities;
using ERDM.Shared.Kernel.Models.Common;
using System.Linq.Expressions;

namespace ERDM.Shared.Kernel.Interfaces
{

    public interface IRepository<T> where T : BaseEntity
    {
        // Basic CRUD
        Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default);

        // Bulk Operations
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        // Query Operations
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Paging
        Task<PaginatedResult<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            string sortBy = null,
            bool sortDescending = false,
            CancellationToken cancellationToken = default);

        // Custom CQL Queries
        Task<IEnumerable<T>> ExecuteQueryAsync(string cql, params object[] parameters);
        Task<T> ExecuteSingleQueryAsync(string cql, params object[] parameters);
        Task<int> ExecuteNonQueryAsync(string cql, params object[] parameters);

        // Batch Operations
        Task ExecuteBatchAsync(IEnumerable<BatchOperation> operations, CancellationToken cancellationToken = default);
    }
}
