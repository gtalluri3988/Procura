namespace DB.EFModel
{
    // Per-asset bid price from a vendor. Header record is TenderVendorSubmission.
    public class BidderSubmissionItem : BaseEntity
    {
        public int Id { get; set; }
        public int TenderVendorSubmissionId { get; set; }
        public int BiddingAssetId { get; set; }
        public decimal BidPrice { get; set; }

        public TenderVendorSubmission? TenderVendorSubmission { get; set; }
        public BiddingAsset? BiddingAsset { get; set; }
    }
}
