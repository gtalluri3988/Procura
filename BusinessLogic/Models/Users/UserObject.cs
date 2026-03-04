using DB.EFModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Users
{
    public class UserObject
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string StaffId { get; set; }
        public int SiteLevelId { get; set; }   // e.g. Ibu Pejabat (HQ)
        public int SiteOffice { get; set; }  // e.g. Kuala Lumpur
        public string Designation { get; set; }

        public string ROCNo { get; set; }

        public bool IsOpeningCommittee { get; set; }

        public bool IsEvaluationCommittee { get; set; }

        public bool IsNegotiationCommittee { get; set; }





        public int CommunityId { get; set; }
        public string? CommunityName { get; set; }

        public int RoleId { get; set; }

        public bool? IsFirstTimeLogin { get; set; }

    }
}
