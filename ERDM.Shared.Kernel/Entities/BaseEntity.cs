using ERDM.Shared.Kernel.Interfaces;

namespace ERDM.Shared.Kernel.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // Domain Events
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        // Audit Helpers
        public void MarkAsCreated(long createdBy)
        {
            CreatedOn = DateTimeOffset.UtcNow;
            CreatedBy = createdBy;
            IsActive = true;
        }

        public void MarkAsModified(long modifiedBy)
        {
            ModifiedOn = DateTimeOffset.UtcNow;
            ModifiedBy = modifiedBy;
        }

        public void MarkAsDeleted()
        {
            IsActive = false;
            ModifiedOn = DateTimeOffset.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            ModifiedOn = DateTimeOffset.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            ModifiedOn = DateTimeOffset.UtcNow;
        }

        // Equality
        public override bool Equals(object obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !Equals(left, right);
        }
    }
}
