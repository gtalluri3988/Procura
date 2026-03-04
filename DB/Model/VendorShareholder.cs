using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorShareholder
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

       
        //public Vendor Vendor { get; set; }

        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string IdentificationNo { get; set; }

        public bool IsBumiputera { get; set; }

        public decimal SharePercentage { get; set; }
        public decimal ShareAmount { get; set; }

        public string FpmsbRelationshipStatus { get; set; }
        public string FeldaSettlerStatus { get; set; }
    }
}
