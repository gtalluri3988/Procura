using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderEvaluationCriteria:BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int JobCategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public int PassingMarks { get; set; }
        public ICollection<TenderEvaluationSpecification>? Specifications { get; set; }
        public JobCategory? JobCategory { get; set; }
        public TenderApplication? Tender { get; set; }

    }
}
