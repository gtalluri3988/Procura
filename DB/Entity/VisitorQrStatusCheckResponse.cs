using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VisitorQrStatusCheckResponse
    {
        public string CommunityId { get; set; } = string.Empty;
        public string QrCodeString { get; set; } = string.Empty;
        public bool IsActiveVisitor { get; set; }
    }
}
