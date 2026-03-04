using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DB.Entity
{
    public class VendorMemberDto
    {
        public List<VendorShareholder>? Shareholders { get; set; }
        public List<VendorDirector>? Directors { get; set; }
        public List<VendorStaffDeclaration>? StaffDeclarations { get; set; }
        public VendorManpower? VendorManpower { get; set; }
    }
}
