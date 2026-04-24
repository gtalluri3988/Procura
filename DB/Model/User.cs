
namespace DB.EFModel
{
    public class User 
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string StaffId { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }
        

        [Required]
        [MaxLength(150)]
        public string EmailAddress { get; set; }

        [MaxLength(20)]
        public string MobileNo { get; set; }

        [ForeignKey(nameof(SiteLevel))]
        public int SiteLevelId { get; set; }   // e.g. Ibu Pejabat (HQ)

        [ForeignKey(nameof(State))]
        public int SiteOffice { get; set; }  // e.g. Kuala Lumpur

        [MaxLength(150)]
        public int? DesignationId { get; set; }


        [Required]
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }

        public bool IsOpeningCommittee { get; set; }

        public bool IsEvaluationCommittee { get; set; }

        public bool IsNegotiationCommittee { get; set; }

        // Audit fields (recommended)
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFirstTimeLogin { get; set; } = false;

        public DateTime? LastLogin { get; set; }

        public Role? Role { get; set; }
        public State? State { get; set; }
        public SiteLevel? SiteLevel { get; set; }
        public Designation? Designation { get; set; }

    }
}
