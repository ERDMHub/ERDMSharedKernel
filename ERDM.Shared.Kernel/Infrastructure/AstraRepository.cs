using Cassandra;
using Cassandra.Mapping;
using ERDM.Shared.Kernel.Entities;
using ERDM.Shared.Kernel.Interfaces;
using ERDM.Shared.Kernel.Models.Common;
using ERDM.Shared.Kernel.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace ERDM.Shared.Kernel.Infrastructure
{
    public class AstraRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        protected readonly ISession _session;
        protected readonly IMapper _mapper;
        protected readonly ILogger<AstraRepository<T>> _logger;
        protected readonly CassandraSettings _settings;
        protected readonly string _tableName;

        public AstraRepository(
            ISession session,
            IOptions<CassandraSettings> settings,
            ILogger<AstraRepository<T>> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _mapper = new Mapper(session);
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tableName = typeof(T).Name.ToLower();
        }

        // Basic CRUD Operations
        public virtual async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("GetById: {TableName}, Id: {Id}", _tableName, id);
                return await _mapper.FirstOrDefaultAsync<T>(
                    $"SELECT * FROM {_tableName} WHERE id = ?", id)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity from {TableName} with id {Id}", _tableName, id);
                throw new InvalidOperationException($"Error retrieving entity from table {_tableName}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("GetAll: {TableName}", _tableName);
                return await _mapper.FetchAsync<T>($"SELECT * FROM {_tableName}")
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities from {TableName}", _tableName);
                throw new InvalidOperationException($"Error retrieving all entities from table {_tableName}", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("GetActive: {TableName}", _tableName);
                return await _mapper.FetchAsync<T>(
                    $"SELECT * FROM {_tableName} WHERE isactive = ? ALLOW FILTERING", true)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active entities from {TableName}", _tableName);
                throw new InvalidOperationException($"Error retrieving active entities from table {_tableName}", ex);
            }
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity.CreatedOn == null)
                    entity.MarkAsCreated(entity.CreatedBy ?? 0);

                _logger.LogDebug("Add: {TableName}, Id: {Id}", _tableName, entity.Id);
                await _mapper.InsertAsync(entity).ConfigureAwait(false);

                await PublishDomainEvents(entity, cancellationToken);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity to {TableName}", _tableName);
                throw new InvalidOperationException($"Error adding entity to table {_tableName}", ex);
            }
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                entity.MarkAsModified(entity.ModifiedBy ?? 0);
                _logger.LogDebug("Update: {TableName}, Id: {Id}", _tableName, entity.Id);
                await _mapper.UpdateAsync(entity).ConfigureAwait(false);

                await PublishDomainEvents(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity in {TableName}", _tableName);
                throw new InvalidOperationException($"Error updating entity in table {_tableName}", ex);
            }
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                // Soft delete
                entity.MarkAsDeleted();
                await UpdateAsync(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity from {TableName}", _tableName);
                throw new InvalidOperationException($"Error deleting entity from table {_tableName}", ex);
            }
        }

        public virtual async Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
                await DeleteAsync(entity, cancellationToken);
        }

        // Bulk Operations - Using IMapper's built-in methods
        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var entity in entities)
                {
                    if (entity.CreatedOn == null)
                        entity.MarkAsCreated(entity.CreatedBy ?? 0);
                }

                _logger.LogDebug("AddRange: {TableName}, Count: {Count}", _tableName, entities.Count());
                await _mapper.InsertAsync(entities).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding range to {TableName}", _tableName);
                throw new InvalidOperationException($"Error adding range to table {_tableName}", ex);
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var entity in entities)
                {
                    entity.MarkAsModified(entity.ModifiedBy ?? 0);
                }

                _logger.LogDebug("UpdateRange: {TableName}, Count: {Count}", _tableName, entities.Count());
                await _mapper.UpdateAsync(entities).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range in {TableName}", _tableName);
                throw new InvalidOperationException($"Error updating range in table {_tableName}", ex);
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var entity in entities)
                {
                    entity.MarkAsDeleted();
                }

                _logger.LogDebug("DeleteRange: {TableName}, Count: {Count}", _tableName, entities.Count());
                // Soft delete - use update
                await _mapper.UpdateAsync(entities).ConfigureAwait(false);

                // For hard delete, use:
                // await _mapper.DeleteAsync(entities).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range from {TableName}", _tableName);
                throw new InvalidOperationException($"Error deleting range from table {_tableName}", ex);
            }
        }

        // Query Operations
        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("FindAsync with expression is not supported in Cassandra. Use ExecuteQueryAsync with CQL.");
            throw new NotSupportedException("Cassandra does not support LINQ queries. Use ExecuteQueryAsync with CQL instead.");
        }

        public virtual async Task<T> FindOneAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("FindOneAsync with expression is not supported in Cassandra. Use ExecuteSingleQueryAsync with CQL.");
            throw new NotSupportedException("Cassandra does not support LINQ queries. Use ExecuteSingleQueryAsync with CQL instead.");
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var cql = $"SELECT COUNT(*) FROM {_tableName}";
                var result = await _mapper.FirstOrDefaultAsync<long>(cql).ConfigureAwait(false);
                return (int)result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting entities in {TableName}", _tableName);
                throw new InvalidOperationException($"Error counting entities in table {_tableName}", ex);
            }
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var count = await CountAsync(predicate, cancellationToken);
            return count > 0;
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            string sortBy = null,
            bool sortDescending = false,
            CancellationToken cancellationToken = default)
        {
            // For Cassandra, use specific CQL with pagination
            var cql = $"SELECT * FROM {_tableName} LIMIT {pageSize}";
            var data = await ExecuteQueryAsync(cql);
            var totalCount = await CountAsync(predicate, cancellationToken);

            return new PaginatedResult<T>(data, totalCount, pageNumber, pageSize);
        }

        // Custom CQL Queries
        public virtual async Task<IEnumerable<T>> ExecuteQueryAsync(string cql, params object[] parameters)
        {
            try
            {
                _logger.LogDebug("ExecuteQuery: {Cql}", cql);
                return await _mapper.FetchAsync<T>(cql, parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Cql}", cql);
                throw new InvalidOperationException($"Error executing CQL query: {cql}", ex);
            }
        }

        public virtual async Task<T> ExecuteSingleQueryAsync(string cql, params object[] parameters)
        {
            try
            {
                _logger.LogDebug("ExecuteSingleQuery: {Cql}", cql);
                return await _mapper.FirstOrDefaultAsync<T>(cql, parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing single query: {Cql}", cql);
                throw new InvalidOperationException($"Error executing CQL query: {cql}", ex);
            }
        }

        public virtual async Task<int> ExecuteNonQueryAsync(string cql, params object[] parameters)
        {
            try
            {
                _logger.LogDebug("ExecuteNonQuery: {Cql}", cql);
                var statement = new SimpleStatement(cql, parameters);
                var result = await _session.ExecuteAsync(statement).ConfigureAwait(false);
                return result.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query: {Cql}", cql);
                throw new InvalidOperationException($"Error executing CQL query: {cql}", ex);
            }
        }

        public virtual async Task ExecuteBatchAsync(IEnumerable<BatchOperation> operations, CancellationToken cancellationToken = default)
        {
            try
            {
                var tasks = new List<Task>();

                foreach (var operation in operations)
                {
                    switch (operation.Type)
                    {
                        case BatchOperationType.Insert:
                            tasks.Add(_mapper.InsertAsync(operation.Entity));
                            break;

                        case BatchOperationType.Update:
                            tasks.Add(_mapper.UpdateAsync(operation.Entity));
                            break;

                        case BatchOperationType.Delete:
                            tasks.Add(_mapper.DeleteAsync(operation.Entity));
                            break;

                        case BatchOperationType.Custom:
                            if (string.IsNullOrEmpty(operation.CustomCql))
                                throw new ArgumentException("Custom CQL cannot be null or empty");

                            var statement = new SimpleStatement(operation.CustomCql, operation.Parameters ?? Array.Empty<object>());
                            tasks.Add(_session.ExecuteAsync(statement));
                            break;
                    }
                }

                // Execute all operations in parallel
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing batch operation with {OperationCount} operations", operations?.Count() ?? 0);
                throw new InvalidOperationException("Error executing batch operation", ex);
            }
        }

        // Synchronous methods for interface compatibility
        public virtual void Update(T entity)
        {
            try
            {
                entity.MarkAsModified(entity.ModifiedBy ?? 0);
                _mapper.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity in {TableName}", _tableName);
                throw new InvalidOperationException($"Error updating entity in table {_tableName}", ex);
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                entity.MarkAsDeleted();
                _mapper.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity from {TableName}", _tableName);
                throw new InvalidOperationException($"Error deleting entity from table {_tableName}", ex);
            }
        }

        protected virtual async Task PublishDomainEvents(T entity, CancellationToken cancellationToken)
        {
            var domainEvents = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                _logger.LogDebug("Domain event: {EventType}", domainEvent.GetType().Name);
            }

            await Task.CompletedTask;
        }
    }
}