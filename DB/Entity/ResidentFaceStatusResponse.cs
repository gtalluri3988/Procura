using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ResidentFaceStatusResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public List<ResidentFaceProfile> FaceProfiles { get; set; } = new();
    }

    public class ResidentFaceProfile
    {
        public int ResidentId { get; set; }
       // public string FaceImageBase64 { get; set; } = string.Empty;
        public string MaintenanceBillStatus { get; set; } = string.Empty;
        public List<ResidentImage>? ResidentImage { get; set; }

       
    }
    public  class ResidentImage
    {
        public int ResidentImageId { get; set; }
        public string FaceImageBase64 { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? UnitNo { get; set; }
    }
}
