namespace ERDM.Shared.Kernel.Interfaces
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        Guid EventId { get; }
    }

    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();

        public long? EntityId { get; set; }
        public string EntityType { get; set; }
        public long? TriggeredBy { get; set; }
    }
}
