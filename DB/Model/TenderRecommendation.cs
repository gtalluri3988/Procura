namespace DB.EFModel
{
    public class TenderRecommendation : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int? RecommendedVendorId { get; set; }
        public string? Reason { get; set; }

        public TenderApplication? Tender { get; set; }
        public Vendor? RecommendedVendor { get; set; }
    }
}
