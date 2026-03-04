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
        //public Vendor? Vendor { get; set; }
    }

}
