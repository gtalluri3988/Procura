namespace DB.Entity
{
    public class TenderApprovalWorkflowDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public string Stage { get; set; }
        public int StageOrder { get; set; }
        public int Level { get; set; }
        public int DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
        public string? AssignedUserDepartment { get; set; }
        public string? AssignedUserDesignation { get; set; }
        public string? AssignedUserMobile { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? ChangeRemarks { get; set; }
        public DateTime? ActionDateTime { get; set; }
    }
}
