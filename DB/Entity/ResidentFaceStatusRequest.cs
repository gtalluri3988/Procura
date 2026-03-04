using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ResidentFaceStatusRequest
    {
        /// <summary>
        /// Unique community identifier (assigned by CSA)
        /// </summary>
        public string CommunityId { get; set; } = string.Empty;
    }
}
