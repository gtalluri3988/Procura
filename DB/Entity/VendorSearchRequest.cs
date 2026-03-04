using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorSearchRequest
    {
        public int? VendorTypeId { get; set; }
        public int? StateId { get; set; }
        public int? VendorCodeStatusId { get; set; }

    }
}
