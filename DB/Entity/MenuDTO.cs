using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MenuDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? Name { get; set; }
        public string? ModuleId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string? ParentId { get; set; }
        public string? Url { get; set; }
        public Boolean Status { get; set; }
        public int SeqId { get; set; }
    }
}
