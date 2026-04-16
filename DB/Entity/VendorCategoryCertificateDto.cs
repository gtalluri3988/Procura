using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorCategoryCertificateDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string? CertificatePath { get; set; }
        public string? FileName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CodeMasterId { get; set; }
        [JsonIgnore]
        public Vendor? Vendor { get; set; }

        [JsonIgnore]
        public CodeMaster? CodeMaster { get; set; }
    }
}
