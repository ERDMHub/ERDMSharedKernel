using ERDM.Shared.Kernel.Constants;

namespace ERDM.Shared.Kernel.Settings
{
    public class CassandraSettings
    {
        public string ContactPoint { get; set; } = "localhost";
        public int Port { get; set; } = AstraKeyNames.DefaultPort;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Keyspace { get; set; } = AstraKeyNames.DefaultKeyspace;
        public string Datacenter { get; set; } = AstraKeyNames.DefaultDatacenter;

        // Consistency Settings
        public string ConsistencyLevel { get; set; } = AstraKeyNames.ConsistencyLocalQuorum;

        // Astra DB Secure Bundle (Base64 encoded)
        public string SecureBundleBase64 { get; set; }

        // Advanced Connection Settings
        public int ConnectTimeoutMillis { get; set; } = AstraKeyNames.DefaultConnectTimeout;
        public int ReadTimeoutMillis { get; set; } = AstraKeyNames.DefaultReadTimeout;
        public int MaxConnections { get; set; } = 10;
        public int CoreConnections { get; set; } = 2;
        public int HeartbeatIntervalSeconds { get; set; } = 30;
        public int RetryAttempts { get; set; } = 3;

        // Query Settings
        public int DefaultPageSize { get; set; } = 50;
        public int MaxPageSize { get; set; } = 1000;

        // Convert string consistency to Cassandra enum
        public Cassandra.ConsistencyLevel GetConsistencyLevel()
        {
            return ConsistencyLevel?.ToLowerInvariant() switch
            {
                "any" => Cassandra.ConsistencyLevel.Any,
                "one" => Cassandra.ConsistencyLevel.One,
                "two" => Cassandra.ConsistencyLevel.Two,
                "three" => Cassandra.ConsistencyLevel.Three,
                "quorum" => Cassandra.ConsistencyLevel.Quorum,
                "all" => Cassandra.ConsistencyLevel.All,
                "localquorum" => Cassandra.ConsistencyLevel.LocalQuorum,
                "eachquorum" => Cassandra.ConsistencyLevel.EachQuorum,
                "serial" => Cassandra.ConsistencyLevel.Serial,
                "localserial" => Cassandra.ConsistencyLevel.LocalSerial,
                "localone" => Cassandra.ConsistencyLevel.LocalOne,
                _ => Cassandra.ConsistencyLevel.LocalQuorum
            };
        }

        // Get masked connection string for logging
        public string GetMaskedConnectionString()
        {
            return $"AstraDB: Keyspace={Keyspace}, ContactPoint={ContactPoint}, Port={Port}, Consistency={ConsistencyLevel}";
        }

        // Validate settings
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ContactPoint) &&
                   Port > 0 &&
                   !string.IsNullOrWhiteSpace(Keyspace);
        }
    }
}
