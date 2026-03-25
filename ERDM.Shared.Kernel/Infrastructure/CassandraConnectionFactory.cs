using Cassandra;
using ERDM.Shared.Kernel.Interfaces;
using ERDM.Shared.Kernel.Settings;
using Microsoft.Extensions.Logging;

namespace ERDM.Shared.Kernel.Infrastructure
{
    public class CassandraConnectionFactory : ICassandraConnectionFactory
    {
        private readonly CassandraSettings _settings;
        private readonly ILogger<CassandraConnectionFactory> _logger;
        private ICluster _cluster;
        private ISession _session;
        private readonly object _lock = new();

        public CassandraConnectionFactory(CassandraSettings settings, ILogger<CassandraConnectionFactory> logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ISession> CreateSessionAsync()
        {
            if (_session != null)
                return _session;

            lock (_lock)
            {
                if (_session != null)
                    return _session;

                try
                {
                    _logger.LogInformation("Creating Cassandra session for keyspace: {Keyspace}", _settings.Keyspace);

                    var clusterBuilder = Cluster.Builder()
                        .AddContactPoint(_settings.ContactPoint)
                        .WithPort(_settings.Port)
                        .WithCredentials(_settings.Username, _settings.Password)
                        .WithDefaultKeyspace(_settings.Keyspace)
                        .WithQueryOptions(new QueryOptions()
                            .SetConsistencyLevel(_settings.GetConsistencyLevel()))
                        .WithRetryPolicy(new DefaultRetryPolicy())
                        .WithReconnectionPolicy(new ConstantReconnectionPolicy(1000))
                        .WithSocketOptions(new SocketOptions()
                            .SetConnectTimeoutMillis(_settings.ConnectTimeoutMillis)
                            .SetReadTimeoutMillis(_settings.ReadTimeoutMillis))
                        .WithPoolingOptions(new PoolingOptions()
                            .SetCoreConnectionsPerHost(HostDistance.Local, _settings.CoreConnections)
                            .SetMaxConnectionsPerHost(HostDistance.Local, _settings.MaxConnections)
                            .SetHeartBeatInterval(_settings.HeartbeatIntervalSeconds * 1000));

                    // Handle secure bundle if provided
                    if (!string.IsNullOrEmpty(_settings.SecureBundleBase64))
                    {
                        var bundlePath = ExtractSecureBundle();
                        clusterBuilder.WithCloudSecureConnectionBundle(bundlePath);
                    }

                    _cluster = clusterBuilder.Build();
                    _session = _cluster.Connect(_settings.Keyspace);

                    _logger.LogInformation("Cassandra session created successfully");
                    return _session;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create Cassandra session");
                    throw;
                }
            }
        }

        public ICluster GetCluster()
        {
            return _cluster;
        }

        private string ExtractSecureBundle()
        {
            var bundleBytes = Convert.FromBase64String(_settings.SecureBundleBase64);
            var tempPath = Path.Combine(Path.GetTempPath(), $"astra-bundle-{Guid.NewGuid()}.zip");
            File.WriteAllBytes(tempPath, bundleBytes);
            return tempPath;
        }
    }
}
