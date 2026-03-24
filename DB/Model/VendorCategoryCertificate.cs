using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorCategoryCertificate:BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public int CodeMasterId { get; set; }
        public string? CertificatePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CodeMaster? CodeMaster { get; set; }
        [JsonIgnore]
        public Vendor? Vendor { get; set; }
    }
}
