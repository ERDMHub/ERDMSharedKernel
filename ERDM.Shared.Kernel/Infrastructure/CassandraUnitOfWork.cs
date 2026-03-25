using Cassandra;
using ERDM.Shared.Kernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERDM.Shared.Kernel.Infrastructure
{
    public class CassandraUnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private readonly ILogger<CassandraUnitOfWork> _logger;
        private bool _disposed;
        private bool _hasActiveTransaction;

        public CassandraUnitOfWork(ISession session, ILogger<CassandraUnitOfWork> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool HasActiveTransaction => _hasActiveTransaction;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // In Cassandra, operations are auto-committed
            // This method is for interface compatibility
            _logger.LogDebug("SaveChangesAsync called - Cassandra operations are auto-committed");
            await Task.CompletedTask;
            return 0;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Cassandra does not support traditional transactions. Use batches for atomic operations.");
            _hasActiveTransaction = true;
            await Task.CompletedTask;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Commit transaction called");
            _hasActiveTransaction = false;
            await Task.CompletedTask;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Cassandra does not support rollback. Use batches for atomic operations.");
            _hasActiveTransaction = false;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Cleanup if needed
            }
            _disposed = true;
        }
    }
}