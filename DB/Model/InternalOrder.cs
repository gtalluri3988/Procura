using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class InternalOrder:BaseEntity
    {
        public int Id { get; set; }

        public string OrderCode { get; set; }     // C88401001
        public string Description { get; set; }   // sg Tiram Pkt 001

        public bool IsActive { get; set; } = true;
    }
}
