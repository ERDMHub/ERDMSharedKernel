
namespace ERDM.Shared.Kernel.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, long id)
            : base($"Entity '{entityName}' with id {id} was not found.")
        {
            EntityName = entityName;
            EntityId = id;
        }

        public NotFoundException(string entityName, string property, object value)
            : base($"Entity '{entityName}' with {property} '{value}' was not found.")
        {
            EntityName = entityName;
        }

        public string EntityName { get; }
        public long? EntityId { get; }
    }
}
