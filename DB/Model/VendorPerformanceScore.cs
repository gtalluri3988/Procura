namespace DB.EFModel
{
    public class VendorPerformanceScore : BaseEntity
    {
        public int Id { get; set; }
        public int VendorPerformanceId { get; set; }
        public string Category { get; set; } = string.Empty;   // Quality / Schedule / Cost / Service / Risk/Safety
        public int Rating { get; set; }                        // 1 - 5
        public int Score { get; set; }                         // Weightage * Rating / 5

        public VendorPerformance? VendorPerformance { get; set; }
    }
}
