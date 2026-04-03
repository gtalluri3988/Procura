using System;
using System.Collections.Generic;

namespace DB.Entity
{
    // ── Fixed performance criteria (hardcoded master list) ─────────────────────
    public static class VendorPerformanceCriteriaList
    {
        public static readonly List<VendorPerformanceCriteriaDefinition> Items = new()
        {
            new() { Category = "Quality",     Indicator = "Work met all technical specs and standards.",  Weightage = 30 },
            new() { Category = "Schedule",    Indicator = "Milestones and final delivery were on time.",  Weightage = 25 },
            new() { Category = "Cost",        Indicator = "Stayed within budget; no additional fees.",    Weightage = 20 },
            new() { Category = "Service",     Indicator = "Proactive communication and problem-solving.", Weightage = 15 },
            new() { Category = "Risk/Safety", Indicator = "Complied with all legal/safety protocols.",    Weightage = 10 },
        };
    }

    public class VendorPerformanceCriteriaDefinition
    {
        public string Category { get; set; } = string.Empty;
        public string Indicator { get; set; } = string.Empty;
        public int Weightage { get; set; }
    }

    // ── Fixed stakeholder feedback questions (hardcoded) ───────────────────────
    public static class VendorPerformanceFeedbackList
    {
        public static readonly List<string> Questions = new()
        {
            "The vendor demonstrated a high level of expertise in their field?",
            "The vendor was responsive to emails/calls (within 24 hours)",
            "The vendor was adaptable when project requirements changed?",
            "Proactive communication and problem-solving.",
            "The vendor's documentation (reports, invoices, manuals) was clear and accurate?"
        };

        public static readonly List<string> ScaleLabels = new()
        {
            "1 - Strongly Disagree",
            "2 - Disagree",
            "3 - Neutral",
            "4 - Agree",
            "5 - Strongly Agree"
        };
    }

    // ── Performance criteria row ───────────────────────────────────────────────
    public class VendorPerformanceCriteriaRowDto
    {
        public string Category { get; set; } = string.Empty;
        public string Indicator { get; set; } = string.Empty;
        public int Weightage { get; set; }
        public int Rating { get; set; }     // 1-5, user selects
        public int Score { get; set; }      // Weightage * Rating / 5 (computed)
    }

    // ── Stakeholder feedback row ───────────────────────────────────────────────
    public class VendorPerformanceFeedbackRowDto
    {
        public int QuestionOrder { get; set; }
        public string Description { get; set; } = string.Empty;
        public int FeedbackScore { get; set; }      // 1-5
        public string FeedbackLabel { get; set; } = string.Empty;  // e.g. "4 - Agree"
    }

    // ── Reviewed By section ────────────────────────────────────────────────────
    public class VendorPerformanceReviewerDto
    {
        public string? PICName { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? MobileNo { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }

    // ── Full Vendor Performance Page (GET) ─────────────────────────────────────
    public class VendorPerformancePageDto
    {
        public int TenderId { get; set; }
        public string? VendorName { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public DateTime? AwardDate { get; set; }
        public string? ReviewMonthYear { get; set; }    // user-editable, e.g. "12/2025"

        public List<VendorPerformanceCriteriaRowDto> PerformanceScores { get; set; } = new();
        public int TotalScore { get; set; }

        public List<VendorPerformanceFeedbackRowDto> StakeholderFeedbacks { get; set; } = new();

        public VendorPerformanceReviewerDto Reviewer { get; set; } = new();
    }

    // ── Save Vendor Performance (POST) ─────────────────────────────────────────
    public class SaveVendorPerformanceDto
    {
        public int TenderId { get; set; }
        public string? ReviewMonthYear { get; set; }
        public List<SaveVendorPerformanceScoreDto> Scores { get; set; } = new();
        public List<SaveVendorPerformanceFeedbackDto> Feedbacks { get; set; } = new();
    }

    public class SaveVendorPerformanceScoreDto
    {
        public string Category { get; set; } = string.Empty;
        public int Rating { get; set; }     // 1-5
    }

    public class SaveVendorPerformanceFeedbackDto
    {
        public int QuestionOrder { get; set; }
        public int FeedbackScore { get; set; }  // 1-5
    }
}
