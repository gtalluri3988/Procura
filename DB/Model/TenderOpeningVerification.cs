namespace DB.EFModel
{
    public class TenderOpeningVerification : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int VerifiedByUserId { get; set; }
        public DateTime VerifiedAt { get; set; }
        public string? Remarks { get; set; }

        public TenderApplication? Tender { get; set; }
        public User? VerifiedByUser { get; set; }
    }
}
