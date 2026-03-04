using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ResidentAccessHistoryDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? VehicleNo { get; set; }

        public string? LevelNo { get; set; }

        public string? BlockNo { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? HouseNo { get; set; }

        public string? RoadNo { get; set; }


        public string? QRImageData { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? ResidentId { get; set; }  // Primary Key
        
        public Boolean TemporaryAccess { get; set; }
        public int CommunityId { get; set; }

        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
    }
}
