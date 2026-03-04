using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorCreditFacility
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public string SupplierName { get; set; }
        public decimal CreditValue { get; set; }

        //public Vendor? Vendor { get; set; }
    }
}
