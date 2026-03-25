namespace ERDM.Shared.Kernel.Exceptions
{
    public class ConcurrencyException : DomainException
    {
        public ConcurrencyException(string entityName, long id)
            : base($"Concurrency conflict on entity '{entityName}' with id {id}")
        {
            EntityName = entityName;
            EntityId = id;
        }

        public string EntityName { get; }
        public long EntityId { get; }
    }
}
