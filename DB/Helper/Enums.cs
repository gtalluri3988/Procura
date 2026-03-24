using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Helper
{

    public enum VendorStatus
    {
        [Display(Name = "Draft")]
        Draft,

        [Display(Name = "Submitted for Approval")]
        Submitted,

        [Display(Name = "Under Review Process")]
        UnderReview,

        [Display(Name = "Approved")]
        Approved,

        [Display(Name = "Rejected by Admin")]
        Rejected,

        [Display(Name = "Pending Approval")]
        PendingApproval,
        [Display(Name = "Pending Request")]
        PendingRequest,
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Expired")]
        Expired,
        [Display(Name = "Blacklisted")]
        Blacklisted

    }

    public enum MemberType
    {
        Shareholder,
        Director
    }

    public enum CategoryType
    {
        FPMSB,
        MOF
    }

    public enum VendorRegistrationStep
    {
        CreateAccount,
        Profile,
        Members,
        Financial,
        Category,
        Experience,
        Declaration,
        Payment
    }

    public enum TenderApplicationStatusEnum
    {
        New = 1,
        Approved = 2,
        Advertised = 3,
        Opening = 4,
        Evaluation = 5,
        Award = 6
    }

    public enum AnnouncementType
    {
        News = 1,
        Advertisement = 2,
        AwardResult = 3
    }
}
