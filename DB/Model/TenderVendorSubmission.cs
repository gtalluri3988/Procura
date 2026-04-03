using System;

namespace DB.EFModel
{
    public class TenderVendorSubmission : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public int SequenceNo { get; set; }                          // 1, 2, 3 → used in "1/3" code
        public decimal? OfferedPrice { get; set; }
        public string TenderOpeningStatus { get; set; } = "Pending"; // Pending / Passed / Failed

        public TenderApplication? Tender { get; set; }
        public Vendor? Vendor { get; set; }
    }
}
