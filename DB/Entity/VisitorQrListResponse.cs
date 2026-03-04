using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VisitorQrListResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public List<VisitorQrProfile> VisitorQrs { get; set; } = new();
    }

    public class VisitorQrProfile
    {
        public int VisitorId { get; set; }
        public string QrCodeString { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }

        public string? VehiclePlateNo { get; set; }


    }


    public class VisitorPlateProfile
    {
        public int VisitorId { get; set; }
        public string PlateNo { get; set; } = string.Empty;
        public string? Entry { get; set; }
        public string? Exit { get; set; }
        public bool IsActive { get; set; }

        public string? PaymentStatus { get; set; }
    }
}
