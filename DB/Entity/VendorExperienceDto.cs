using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorExperienceDto
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
    }
}
