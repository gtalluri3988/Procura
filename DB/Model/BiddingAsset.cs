namespace DB.EFModel
{
    public class BiddingAsset : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int SequenceNo { get; set; }
        public string? ProjectState { get; set; }       // e.g. "Trolak Utara"
        public string? AssetDetails { get; set; }       // e.g. "Toyota Hilux"
        public string? AssetRefNo { get; set; }         // e.g. "WVH 8935"
        public decimal StartingPrice { get; set; }
        public int? YearPurchased { get; set; }

        public TenderApplication? Tender { get; set; }
        public ICollection<BidderSubmissionItem>? SubmissionItems { get; set; }
    }
}
