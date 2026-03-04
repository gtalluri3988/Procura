using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Model
{
    public class CodeSystem
    {
        public int Id { get; set; }
        public string Code { get; set; }  // FPMSB, MOF, CIDB
        public string Name { get; set; }

        public ICollection<CodeHierarchy> Codes { get; set; }
    }
}
