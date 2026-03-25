namespace ERDM.Shared.Kernel.DTOs
{
    public class FilterDto : PaginationDto
    {
        public string SearchTerm { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool? IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public string Status { get; set; }
    }
}
