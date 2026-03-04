using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MaintenanceStatusResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public string? OverdueAccess { get; set; } 
        public List<VehicleProfile> VehicleProfiles { get; set; } = new();
    }

    public class VehicleProfile
    {
        public string VehiclePlateNo { get; set; } = string.Empty;
        public string MaintenanceBillStatus { get; set; } = string.Empty;
        
        
    }
}
