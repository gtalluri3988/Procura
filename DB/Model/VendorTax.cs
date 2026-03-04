using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorTax
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public string TaxType { get; set; }
        public string SstNo { get; set; }
        public DateTime? SstRegistrationDate { get; set; }
        public string TinNo { get; set; }
        public string MsicCode { get; set; }

        //public Vendor? Vendor { get; set; }
    }

}
