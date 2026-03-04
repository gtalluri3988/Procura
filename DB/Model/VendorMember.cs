using DB.EFModel;
using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorMember : BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }

        public MemberType MemberType { get; set; } // Shareholder / Director

        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string IdentificationNo { get; set; }
        public string Designation { get; set; }

        public bool IsBumiputera { get; set; }

        public decimal? SharePercentage { get; set; }
        public decimal? ShareAmount { get; set; }

        public bool IsFpmSbStaff { get; set; }
        public bool IsFamilyOfFpmSbStaff { get; set; }


    }

}
