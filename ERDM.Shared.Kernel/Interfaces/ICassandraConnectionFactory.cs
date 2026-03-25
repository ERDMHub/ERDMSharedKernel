using Cassandra;


namespace ERDM.Shared.Kernel.Interfaces
{
    public interface ICassandraConnectionFactory
    {
        Task<ISession> CreateSessionAsync();
        ICluster GetCluster();
    }
}
