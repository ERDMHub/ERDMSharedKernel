using Cassandra;
using Cassandra.Mapping;
using ERDM.Shared.Kernel.Infrastructure;
using ERDM.Shared.Kernel.Interfaces;
using ERDM.Shared.Kernel.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ERDM.Shared.Kernel.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAstraDb(this IServiceCollection services, IConfiguration configuration)
        {
            var cassandraSettings = configuration.GetSection("Cassandra").Get<CassandraSettings>();
            if (cassandraSettings == null)
                throw new InvalidOperationException("Cassandra settings not found in configuration");

            // Build cluster with Astra DB secure bundle support
            var cluster = Cluster.Builder()
                .AddContactPoint(cassandraSettings.ContactPoint)
                .WithPort(cassandraSettings.Port)
                .WithCredentials(cassandraSettings.Username, cassandraSettings.Password)
                .WithDefaultKeyspace(cassandraSettings.Keyspace)
                .WithQueryOptions(new QueryOptions()
                    .SetConsistencyLevel(GetConsistencyLevel(cassandraSettings.ConsistencyLevel)))
                .WithRetryPolicy(new DefaultRetryPolicy())
                .WithReconnectionPolicy(new ConstantReconnectionPolicy(1000))
                .WithSocketOptions(new SocketOptions()
                    .SetConnectTimeoutMillis(30000)
                    .SetReadTimeoutMillis(120000))
                .Build();

            var session = cluster.Connect(cassandraSettings.Keyspace);

            services.AddSingleton<ICluster>(cluster);
            services.AddSingleton<Cassandra.ISession>(session);
            services.AddSingleton<IMapper>(sp => new Mapper(sp.GetRequiredService<Cassandra.ISession>()));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(AstraRepository<>));
            services.AddScoped<IUnitOfWork, CassandraUnitOfWork>();

            return services;
        }

        public static IServiceCollection AddERDMKernel(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAstraDb(configuration);
            services.AddRepositories();

            return services;
        }

        private static Cassandra.ConsistencyLevel GetConsistencyLevel(string level)
        {
            return level?.ToLower() switch
            {
                "localquorum" => ConsistencyLevel.LocalQuorum,
                "quorum" => ConsistencyLevel.Quorum,
                "localone" => ConsistencyLevel.LocalOne,
                "one" => ConsistencyLevel.One,
                "any" => ConsistencyLevel.Any,
                _ => ConsistencyLevel.LocalQuorum
            };
        }
    }
}
