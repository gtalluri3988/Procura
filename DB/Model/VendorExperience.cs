using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorExperience
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public string ProjectName { get; set; }
        public string Organization { get; set; }
        public decimal ProjectValue { get; set; }
        public string Status { get; set; } // Completed / OnGoing
        public int CompletionYear { get; set; }

        public string AttachmentPath { get; set; }
        public string? FileName { get; set; }
        [JsonIgnore]
        public Vendor? Vendor { get; set; }
    }

}
