using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VisitorQrListRequest
    {
        public string CommunityId { get; set; } = string.Empty;
        public string? VehiclePlateNo { get; set; } = string.Empty;
    }
}
