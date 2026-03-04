using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderDocument:BaseEntity
    {
        public int Id { get; set; }
        public int JobCategoryId { get; set; }
        public string DocumentName { get; set; }
        public string RequirementType { get; set; } // Mandatory / Optional
        public bool IsActive { get; set; } = true;
        public JobCategory? JobCategory { get; set; }
    }
}
