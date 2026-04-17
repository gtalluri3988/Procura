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
        public string? TenderCode { get; set; }
        public int? ApplicationLevelId { get; set; }
        public string? ProjectName { get; set; }
        public int JobCategoryId { get; set; }
        public int TenderCategoryId { get; set; }
        public DateTime? EstimatedJobStartDate { get; set; }
        public bool DepositRequired { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
        public decimal? EstimatedPrices { get; set; }
        public int? MinCapitalPercent { get; set; }
        public decimal? MinCapitalAmount { get; set; }
        public int? TenderApplicationStatusId { get; set; }

        public int TenderCreatedBy { get; set; }
        public ICollection<TenderJobScope>? JobScopes { get; set; }
        public ICollection<TenderCategoryCode>? TenderCategoryCodes { get; set; }
        public TenderCategory? TenderCategory { get; set; }
        public JobCategory? JobCategory { get; set; }
        public TenderSiteVisit? TenderSiteVisit { get; set; }
        public ICollection<TenderRequiredDocument>? Documents { get; set; }
        public ICollection<TenderReview>? Reviews { get; set; }
        public ICollection<TenderApproval>? Approvals { get; set; }
        public ICollection<TenderApprovalWorkflow>? ApprovalWorkflows { get; set; }

        public TenderApplicationStatus? TenderApplicationStatus { get; set; }
       

        [ForeignKey(nameof(TenderCreatedBy))]
        public User? TenderCreatedByUser { get; set; }
    }
}
