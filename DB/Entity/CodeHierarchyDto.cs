using DB.EFModel;
using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CodeHierarchyDto
    {
        public int? Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool? IsActive { get; set; }
        public List<CodeHierarchyDto> Children { get; set; } = new();



    }
}
