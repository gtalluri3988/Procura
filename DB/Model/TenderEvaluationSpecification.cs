using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderEvaluationSpecification:BaseEntity
    {
        public int Id { get; set; }
        public int CriteriaId { get; set; }

        public string? Specification { get; set; }
        public int Weightage { get; set; }

        public TenderEvaluationCriteria? Criteria { get; set; }
    }
}
