using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorManagementSettingDto
    {
        public int Id { get; set; }

        // Registration
        public decimal? RegistrationFee { get; set; }          // RM
        public int RegistrationValidityYears { get; set; }    // Years

        public decimal? PurchaseFee { get; set; }

        // Renewal
        public decimal? RenewalFee { get; set; }
        public decimal? LateRenewalFee { get; set; }

        // Category Change
        public decimal CategoryCodeChangeFee { get; set; }

        // Certificate
        public string? CertificateBackgroundImagePath { get; set; }

        // Blacklist Rule
        public int BlacklistDenyDurationMonths { get; set; }
    }
}
