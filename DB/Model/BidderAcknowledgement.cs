namespace DB.EFModel
{
    public class BidderAcknowledgement : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public string Acknowledgement { get; set; } = string.Empty;  // "Accept" / "Reject"
        public string? StampDutyPath { get; set; }
        public string? StampDutyFileName { get; set; }
        public DateTime AcknowledgementDateTime { get; set; }

        public TenderApplication? Tender { get; set; }
        public Vendor? Vendor { get; set; }
    }
}
