using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Vendor
    {
        public int Id { get; set; }

        // Account
        public int CompanyEntityTypeId { get; set; }
        public string RocNumber { get; set; }
        public string PasswordHash { get; set; }

        // Profile
        public string? CompanyName { get; set; }
        public DateTime? DateOfIncorporation { get; set; }

        public string? Address { get; set; }
        public string? Postcode { get; set; }
        public int? StateId { get; set; }
        public string? City { get; set; }
        public int? CountryId { get; set; }

        public string? OfficePhoneNo { get; set; }
        public string? FaxNo { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        public int? IndustryTypeId { get; set; }
        public string? BusinessCoverageArea { get; set; }

        public string? PicName { get; set; }
        public string? PicMobileNo { get; set; }
        public string? PicEmail { get; set; }

        public string? Form24AttachmentPath { get; set; }
        public string? FileName { get; set; }
        public string? VendorCodeStatus { get; set; }
        public string? VendorCode { get; set; }

        public bool? IsRegistrationComplete { get; set; }

        public int? RoleId { get; set; }
        public DateTime? RequestDatetime { get; set; }
        public DateTime? ApprovalDatetime { get; set; }
        public bool Status { get; set; }
        public VendorRegistrationStep? CurrentStep { get; set; }
        = VendorRegistrationStep.CreateAccount;

        public DateTime CreatedDate { get; set; }


        

        public VendorManpower? VendorManpower { get; set; }

        public VendorFinancial? VendorFinancial { get; set; }

        public ICollection<VendorTax>? VendorTaxes { get; set; } = new List<VendorTax>();

        public ICollection<VendorBank>? VendorBanks { get; set; } = new List<VendorBank>();

        public ICollection<VendorCategory> VendorCategories { get; set; } = new List<VendorCategory>();

        public ICollection<VendorExperience>? VendorExperiences { get; set; } = new List<VendorExperience>();

        public ICollection<VendorCategoryCertificate>? VendorCategoryCertificates { get; set; } = new List<VendorCategoryCertificate>();

        public VendorDeclaration? VendorDeclaration { get; set; }

        public PaymentRequest? VendorPayment { get; set; }
        public IndustryType? IndustryType { get; set; }
        public State? State { get; set; }
        public Role? Role { get; set; }
        public CompanyEntityType? CompanyEntityType { get; set; }
        public ICollection<VendorShareholder> Shareholders { get; set; }
       = new List<VendorShareholder>();

        public ICollection<VendorDirector>? Directors { get; set; }
            = new List<VendorDirector>();

        public ICollection<VendorStaffDeclaration>? StaffDeclarations { get; set; }
            = new List<VendorStaffDeclaration>();

        public ICollection<VendorCreditFacility>? CreditFacilities { get; set; }
           = new List<VendorCreditFacility>();


    }

}
