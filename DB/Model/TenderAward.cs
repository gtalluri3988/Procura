namespace DB.EFModel
{
    public class TenderAward : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int? AwardedVendorId { get; set; }
        public decimal? ProjectValue { get; set; }
        public decimal? YearlyExpenses { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public string? Agreement { get; set; }
        public DateTime? AgreementDateSigned { get; set; }
        public string? PONumber { get; set; }

        public TenderApplication? Tender { get; set; }
        public Vendor? AwardedVendor { get; set; }
        public ICollection<TenderAwardMinutesOfMeeting>? MinutesOfMeetings { get; set; }
    }
}
