namespace DB.EFModel
{
    public class TenderTechnicalEvaluationScore : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public int SpecificationId { get; set; }   // → TenderEvaluationSpecification
        public int Score { get; set; }             // 0-5
        public string? Remarks { get; set; }

        public TenderApplication? Tender { get; set; }
        public Vendor? Vendor { get; set; }
        public TenderEvaluationSpecification? Specification { get; set; }
    }
}
