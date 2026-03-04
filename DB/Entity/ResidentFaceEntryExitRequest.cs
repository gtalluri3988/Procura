using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ResidentFaceEntryExitRequest
    {
        /// <summary>
        /// Unique community identifier (assigned by CSA)
        /// </summary>
        public string CommunityId { get; set; } = string.Empty;

        /// <summary>
        /// The face image in Base64 format captured by Microbit device
        /// </summary>
        public string FaceImageBase64 { get; set; } = string.Empty;

        /// <summary>
        /// Entry timestamp (UTC format preferred)
        /// Sent during both entry and exit events
        /// </summary>
        public DateTime EntryTime { get; set; }

        /// <summary>
        /// Exit timestamp (available only during exit event)
        /// </summary>
        public DateTime? ExitTime { get; set; }
    }

    public class VisitorVehicleListRequest
    {
        public string CommunityId { get; set; } = string.Empty;
    }
    public class VisitorVehicleListResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public List<VisitorVehicleProfile> VisitorVehicles { get; set; } = new();
    }

    public class VisitorVehicleProfile
    {
        public string VehiclePlateNo { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }

    public class VisitorVehicleStatusCheckRequest
    {
        public string CommunityId { get; set; } = string.Empty;
        public string VehiclePlateNo { get; set; } = string.Empty;
    }


    public class VisitorVehicleStatusCheckResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public string VehiclePlateNo { get; set; } = string.Empty;
        public bool IsActiveVisitor { get; set; }
    }

    public class VisitorVehicleEntryExitRequest
    {
        public string CommunityId { get; set; } = string.Empty;
        public string VehiclePlateNo { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }

    public class ResidentFaceEntryExit
    {
        public int Id { get; set; }
        public string CommunityId { get; set; } = string.Empty;
        public string FaceImageBase64 { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}
