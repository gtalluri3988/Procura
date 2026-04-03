namespace DB.EFModel
{
    public class TenderAwardMinutesOfMeeting : BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? MeetingOutcome { get; set; }
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }

        public TenderApplication? Tender { get; set; }
    }
}
