
namespace ERDM.Shared.Kernel.Models.Common
{
    public class BatchOperation
    {
        public BatchOperationType Type { get; set; }
        public object Entity { get; set; }
        public string CustomCql { get; set; }
        public object[] Parameters { get; set; }
    }
}
