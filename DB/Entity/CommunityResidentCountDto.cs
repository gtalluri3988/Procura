using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CommunityResidentCountDto
    {
        public int Id { get; set; }
        public string? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        public int  ResidentCount { get; set; }
    }
}
