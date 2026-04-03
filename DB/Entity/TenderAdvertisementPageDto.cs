using System;
using System.Collections.Generic;

namespace DB.Entity
{
    // --- Advertisement Setting ---
    public class TenderAdvertisementSettingDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public DateTime AdvertisementStartDate { get; set; }
        public DateTime AdvertisementEndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime OpeningStartDate { get; set; }
        public DateTime OpeningEndDate { get; set; }
        public DateTime EvaluationStartDate { get; set; }
        public DateTime EvaluationEndDate { get; set; }
        public string? TenderDocumentPath { get; set; }
    }

    // --- Opening Committee ---
    public class TenderOpeningCommitteeDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
    }

    // --- Evaluation Committee ---
    public class TenderEvaluationCommitteeDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
    }

    // --- Evaluation Criteria Specification ---
    public class TenderEvaluationSpecificationDto
    {
        public int Id { get; set; }
        public string? Specification { get; set; }
        public int Weightage { get; set; }
    }

    // --- Evaluation Criteria ---
    public class TenderEvaluationCriteriaDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public int JobCategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public int PassingMarks { get; set; }
        public List<TenderEvaluationSpecificationDto> Specifications { get; set; } = new();
    }

    // --- Issuance Approval ---
    public class TenderIssuenceApprovalDto
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public int ReviewLevel { get; set; }
        public int ReviewerId { get; set; }
        public int? TenderStatusId { get; set; }
        public string? Remarks { get; set; }
        public DateTime ReviewDateTime { get; set; }
        public string? ReviewerName { get; set; }
    }

    // --- Single Page Request/Response ---
    public class TenderAdvertisementPageDto
    {
        public int TenderApplicationId { get; set; }
        public TenderAdvertisementSettingDto? AdvertisementSetting { get; set; }
        public List<TenderOpeningCommitteeDto> OpeningCommittees { get; set; } = new();
        public List<TenderEvaluationCommitteeDto> EvaluationCommittees { get; set; } = new();
        public List<TenderEvaluationCriteriaDto> EvaluationCriterias { get; set; } = new();
        public List<TenderIssuenceApprovalDto> IssuanceApprovals { get; set; } = new();
    }
}
