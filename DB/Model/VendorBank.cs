using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorBank
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }

        public string? BankBranch { get; set; }
        public string? BankBranchAddress { get; set; }
        public decimal? Balance { get; set; }
        public string? FixedDeposit { get; set; }
        public string? Attachment { get; set; }

        //public Vendor? Vendor { get; set; }
    }

}
