using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Model
{
    public class MaterialBudget
    {
        public int Id { get; set; }

        public int JobCategoryId { get; set; }

        public string ServiceCode { get; set; }

        public string ShortText { get; set; }

        public string MaterialGroup { get; set; }

        public string MaterialGroupDescription { get; set; }

        public string Unit { get; set; }

        public string POActAssign { get; set; }

        public string GLAccount { get; set; }

        public string GLDescription { get; set; }

        public string RujukanType { get; set; }    // "WBS" | "COST_CENTRE" | "IO"

        public string RujukanValue { get; set; }

        public decimal Amount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int CreatedBy { get; set; }

        // Navigation Property
        public JobCategory JobCategory { get; set; }
    }
}
