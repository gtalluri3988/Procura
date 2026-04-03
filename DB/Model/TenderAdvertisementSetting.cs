using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DB.EFModel
{
    public class TenderAdvertisementSetting:BaseEntity
    {
        public int Id { get; set; }
        public DateTime AdvertisementStartDate { get; set; }
        public DateTime AdvertisementEndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TenderId { get; set; }
        // Tender Opening Committee
        public DateTime OpeningStartDate { get; set; }
        public DateTime OpeningEndDate { get; set; }   
        public ICollection<TenderOpeningCommittee> OpeningCommittees { get; set; } = new List<TenderOpeningCommittee>();
        // Tender Evaluation
        public DateTime EvaluationStartDate { get; set; }
        public DateTime EvaluationEndDate { get; set; }
        public ICollection<TenderEvaluationCommittee> EvaluationCommittees { get; set; } = new List<TenderEvaluationCommittee>();
        public string? TenderDocumentPath { get; set; }
        public TenderApplication? Tender { get; set; }

       
    }
}
