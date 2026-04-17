using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DB.EFModel
{
    public class TenderApprovalWorkflow : BaseEntity
    {
        public int Id { get; set; }

        public int TenderApplicationId { get; set; }

        /// <summary>"REVIEWER" | "WILAYAH" | "HQ" | "ISSUANCE"</summary>
        public string Stage { get; set; }

        /// <summary>1=REVIEWER, 2=WILAYAH, 3=HQ, 4=ISSUANCE</summary>
        public int StageOrder { get; set; }

        /// <summary>Sub-level within a stage (1, 2, 3)</summary>
        public int Level { get; set; }

        /// <summary>Expected designation for this slot</summary>
        public int DesignationId { get; set; }

        /// <summary>Assigned approver user Id (null = not yet assigned)</summary>
        public int? AssignedUserId { get; set; }

        /// <summary>null=Not Started | "Pending" | "Approved" | "Rejected"</summary>
        public string? Status { get; set; }

        public string? Remarks { get; set; }

        /// <summary>Reason for changing the approver</summary>
        public string? ChangeRemarks { get; set; }

        /// <summary>When the approver took action</summary>
        public DateTime? ActionDateTime { get; set; }

        // Navigation properties
        [JsonIgnore]
        public TenderApplication? TenderApplication { get; set; }

        [ForeignKey(nameof(AssignedUserId))]
        public User? AssignedUser { get; set; }

        [ForeignKey(nameof(DesignationId))]
        public Designation? Designation { get; set; }
    }
}
