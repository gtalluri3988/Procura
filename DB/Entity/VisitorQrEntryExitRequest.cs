using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VisitorQrEntryExitRequest
    {
        public string CommunityId { get; set; } = string.Empty;
        public string QrCodeString { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}
