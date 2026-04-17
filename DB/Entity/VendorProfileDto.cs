using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorProfileDto
    {
        public int Id { get; set; }
        public int CompanyEntityTypeId { get; set; }
        public string RocNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime? DateOfIncorporation { get; set; }

        public string Address { get; set; }
        public string Postcode { get; set; }
        public int? StateId { get; set; }
        public string City { get; set; }
        public int? CountryId { get; set; }

        public string OfficePhoneNo { get; set; }
        public string? FaxNo { get; set; }
        public string Email { get; set; }
        public string? Website { get; set; }

        public int? IndustryTypeId { get; set; }
        public string? BusinessCoverageArea { get; set; }

        public string PicName { get; set; }
        public string PicMobileNo { get; set; }
        public string PicEmail { get; set; }
        public string? VendorCodeStatus { get; set; }
        public string? VendorCode { get; set; }
        public bool? IsRegistrationComplete { get; set; }
        public bool Status { get; set; }
        public DateTime? RequestDatetime { get; set; }
        public DateTime? ApprovalDatetime { get; set; }
        public string? Form24AttachmentPath { get; set; }
        public string? FileName { get; set; }
        public int RoleId { get; set; }
        public VendorRegistrationStep? CurrentStep { get; set; }

        // NEW: suggested next step for the UI to navigate to (null when no next step)
        public VendorRegistrationStep? NextStep { get; set; }
        
    }
}
