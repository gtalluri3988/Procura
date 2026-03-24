using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
   
    public class VendorDirector
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        //public Vendor Vendor { get; set; }

        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string IdentificationNo { get; set; }

        public string Designation { get; set; }

        public bool IsBumiputera { get; set; }
        public string FpmsbRelationshipStatus { get; set; }
    }
}
