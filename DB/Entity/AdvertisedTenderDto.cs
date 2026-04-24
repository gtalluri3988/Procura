namespace DB.Entity
{
    public class AdvertisedTenderDto
    {
        public int Id { get; set; }
        public string? TenderCode { get; set; }
        public string? ProjectName { get; set; }
        public string? TenderCategoryName { get; set; }
        public DateTime? AdvertisementStartDate { get; set; }
        public DateTime? AdvertisementEndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
