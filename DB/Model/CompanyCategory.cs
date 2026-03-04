using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class CompanyCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CompanyEntityType> CompanyEntityType { get; set; }
    }
}
