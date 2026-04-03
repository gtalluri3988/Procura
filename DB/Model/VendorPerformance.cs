namespace DB.EFModel
{
    public class VendorPerformance : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public string? ReviewMonthYear { get; set; }   // e.g. "12/2025"
        public int TotalScore { get; set; }

        // Reviewer info (captured from logged-in user at save time)
        public string? PICName { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? MobileNo { get; set; }

        public TenderApplication? Tender { get; set; }
        public ICollection<VendorPerformanceScore>? Scores { get; set; }
        public ICollection<VendorPerformanceFeedback>? Feedbacks { get; set; }
    }
}
