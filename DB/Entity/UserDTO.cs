using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace DB.Entity
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? StaffId { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public Nullable<int> SiteLevelId { get; set; }
        public Nullable<int> SiteOfficeId { get; set; }
        public int? DesignationId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public bool IsOpeningCommittee { get; set; }
        public bool IsEvaluationCommittee { get; set; }
        public bool IsNegotiationCommittee { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }
        public string? Status { get; set; }       
        public string? RoleName { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string? UserName { get; set; }
        public string? PicturePath { get; set; }
        public Nullable<System.DateTime> PasswordExpiryDate { get; set; }
        public Nullable<int> BadLoginAttempt { get; set; }
        public string? SiteLevelName { get; set; }
        public string? SiteOfficeName { get; set; }
        public string? DesignationName { get; set; }
        [JsonIgnore]
        public SiteLevel? SiteLevel { get; set; }   // e.g. Ibu Pejabat (HQ)
        [JsonIgnore]
        public State? SiteOffice { get; set; }  // e.g. Kuala Lumpur
        [JsonIgnore]
        public Role? Roles { get; set; }
    }
}
