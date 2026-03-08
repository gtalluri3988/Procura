using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderApplication:BaseEntity
    {
        public int Id { get; set; }

        public string ApplicationLevel { get; set; }
        public string ProjectName { get; set; }
        public string JobCategory { get; set; }
        public string TenderCategory { get; set; }

        public DateTime? EstimatedJobStartDate { get; set; }
        public bool DepositRequired { get; set; }
        public decimal DepositAmount { get; set; }

        public string Remarks { get; set; }
        public string Status { get; set; }

        public decimal EstimatedPrices { get; set; }
        public int MinCapitalPercent { get; set; }
        public decimal MinCapitalAmount { get; set; }

        public string CategoryCode { get; set; }

        public bool SiteVisitRequired { get; set; }
        public DateTime? SiteVisitDate { get; set; }
        public string SiteVisitTime { get; set; }
        public string SiteVisitVenue { get; set; }
        public string SiteVisitAttendance { get; set; }
        public string SiteVisitRemarks { get; set; }
        public string SiteVisitForm { get; set; }

        public ICollection<TenderJobScope> JobScopes { get; set; }
        
        public ICollection<TenderRequiredDocument> Documents { get; set; }
        public ICollection<TenderReview> Reviews { get; set; }
        public ICollection<TenderApproval> Approvals { get; set; }
    }
}
