using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CodeMasterHierarchyDto
    {
        public int CodeId { get; set; }

        public string Code { get; set; }

        public List<CategoryDto> Categories { get; set; }
    }
}
