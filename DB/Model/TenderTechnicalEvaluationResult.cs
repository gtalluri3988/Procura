namespace DB.EFModel
{
    public class TenderTechnicalEvaluationResult : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public int TotalScore { get; set; }
        public int PassingMarks { get; set; }
        public string Result { get; set; } = "Pending";  // Pending / Passed / Failed
        public int Ranking { get; set; }

        public TenderApplication? Tender { get; set; }
        public Vendor? Vendor { get; set; }
    }
}
