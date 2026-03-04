using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class JobCategory
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
