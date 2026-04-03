namespace DB.EFModel
{
    public class VendorPerformanceFeedback : BaseEntity
    {
        public int Id { get; set; }
        public int VendorPerformanceId { get; set; }
        public int QuestionOrder { get; set; }    // 1-5, matches fixed question list
        public int FeedbackScore { get; set; }    // 1=Strongly Disagree ... 5=Strongly Agree

        public VendorPerformance? VendorPerformance { get; set; }
    }
}
