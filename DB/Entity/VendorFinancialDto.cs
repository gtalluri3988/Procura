using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorFinancialDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public decimal PaidUpCapital { get; set; }
        public decimal AuthorizedCapital { get; set; }
        public decimal WorkingCapital { get; set; }
        public decimal LiquidCapital { get; set; }
        public decimal AssetBalance { get; set; }

        public decimal BumiputeraEquityAmount { get; set; }
        public decimal BumiputeraEquityPercentage { get; set; }

        public decimal NonBumiputeraEquityAmount { get; set; }
        public decimal NonBumiputeraEquityPercentage { get; set; }

        public decimal RollingCapital { get; set; }
        public decimal TotalOverdraft { get; set; }
        public decimal OthersCredit { get; set; }


        // Attachment
        public string? LatestBankStatementPath { get; set; }
        public string? FileName { get; set; }

        // Navigation
        public ICollection<VendorCreditFacility> CreditFacilities { get; set; }
            = new List<VendorCreditFacility>();

        public VendorTax? Tax { get; set; }
        public VendorBank? Bank { get; set; }
        
    }
}
