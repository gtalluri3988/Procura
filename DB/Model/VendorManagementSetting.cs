using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorManagementSetting:BaseEntity
    {
        public int Id { get; set; }

        // Registration
        public decimal? RegistrationFee { get; set; }          // RM

        public decimal? PurchaseFee { get; set; }
        public int RegistrationValidityYears { get; set; }    // Years

        // Renewal
        public decimal? RenewalFee { get; set; }
        public decimal? LateRenewalFee { get; set; }

        // Category Change
        public decimal CategoryCodeChangeFee { get; set; }

        // Certificate
        public string? CertificateBackgroundImagePath { get; set; }
        public string? FileName { get; set; }

        // Blacklist Rule
        public int BlacklistDenyDurationMonths { get; set; }

        
    }

}
