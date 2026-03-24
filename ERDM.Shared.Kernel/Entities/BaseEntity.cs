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
    }
}
