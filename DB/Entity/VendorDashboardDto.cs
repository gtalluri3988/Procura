using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorDashboardDto
    {
        public int Total { get; set; }
        public int Draft { get; set; }

        public int Approved { get; set; }
        public int Blacklisted { get; set; }

        public int PendingApproval { get; set; }
        //public int Active { get; set; }
        public int Expired { get; set; }

        public IEnumerable<VendorProfileDto> Vendors { get; set; }
        
    }
}
