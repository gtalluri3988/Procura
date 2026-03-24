using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class EvaluationCriteria:BaseEntity
    {
        public int Id { get; set; }
        public int JobCategoryId { get; set; }
        public string Specification { get; set; }
        public int WeightagePercent { get; set; }
        public bool IsActive { get; set; } = true;
        public JobCategory? JobCategory { get; set; }
    }
}
