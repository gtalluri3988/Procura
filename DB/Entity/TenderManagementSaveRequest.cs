using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class TenderManagementSaveRequest
    {
        public TenderSettingDto TenderSetting { get; set; }

        public List<TenderDocumentDto> TenderDocuments { get; set; }

        public List<EvaluationCriteriaDto> EvaluationCriterias { get; set; }
    }
}
