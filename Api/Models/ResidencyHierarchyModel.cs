namespace Api.Models
{
    public class ResidencyHierarchyModel
    {
        public int CommunityId { get; set; }
        public string? TargetField { get; set; }
        public string? RoadNo { get; set; }
        public string? BlockNo { get; set; }
        public string? Level { get; set; }
    }
}
