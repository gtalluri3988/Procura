using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorStaffDeclaration
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        //public Vendor Vendor { get; set; }

        public string FullName { get; set; }
        public string StaffId { get; set; }
        public string IdentificationNo { get; set; }
        public string Designation { get; set; }
        public string CompanyName { get; set; }

        public bool IsFamilyMember { get; set; }
        public bool IsShareholder { get; set; }
        public bool IsBoardOfDirector { get; set; }
    }

}
