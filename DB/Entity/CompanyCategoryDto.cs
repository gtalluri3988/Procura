using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CompanyCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CompanyEntityType> CompanyEntityType { get; set; }
    }
}
