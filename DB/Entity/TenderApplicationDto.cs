using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class TenderApplicationDto
    {
        public int Id { get; set; }
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

        public int TenderCreatedBy { get; set; }
        public ICollection<TenderJobScope>? JobScopes { get; set; }
        public List<TendorCategoryCodeDto>? TendorCategoryCodeDto { get; set; }
        [JsonIgnore]
        public TenderCategory? TenderCategory { get; set; }
        [JsonIgnore]
        public JobCategory? JobCategory { get; set; }
        public TenderSiteVisit? TenderSiteVisit { get; set; }
        public ICollection<TenderRequiredDocument>? Documents { get; set; }
        [JsonIgnore]
        public ICollection<TenderReview>? Reviews { get; set; }
        [JsonIgnore]
        public ICollection<TenderApproval>? Approvals { get; set; }
        public DateTime CreatedDate { get; set; }

        public User? TenderCreatedByUser { get; set; }
    }
}
