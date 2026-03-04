using DB.Helper;
using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorCategory
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public CategoryType CategoryType { get; set; } // FPMSB / MOF

       // Link to master category
        public int MasterCategoryId { get; set; }
        public CodeHierarchy MasterCategory { get; set; }

        public string CertificatePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CodeSystemId { get; set; }  // FPMSB / MOF / CIDB
        //public Vendor? Vendor { get; set; }
    }

}
