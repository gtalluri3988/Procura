using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    [Table("VendorManpowers")]
    public class VendorManpower :BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public int NoOfBumiputera { get; set; }
        public int NoOfNonBumiputera { get; set; }

        public decimal BumiputeraPercentage { get; set; }
        public decimal NonBumiputeraPercentage { get; set; }

        public int TotalManpower { get; set; }

        //public Vendor? Vendor { get; set; }
    }

}
