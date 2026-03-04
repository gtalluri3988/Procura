using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Model
{
    public class CodeHierarchy
    {
        public int Id { get; set; }

        public int CodeSystemId { get; set; }
        public CodeSystem CodeSystem { get; set; }

        public int? ParentId { get; set; }
        public CodeHierarchy Parent { get; set; }
        public ICollection<CodeHierarchy> Children { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }

        public int Level { get; set; }   // 1,2,3
        public bool? IsActive { get; set; }

        public ICollection<VendorCategory> VendorCategories { get; set; }


    }
}
