using System.Collections.Generic;

namespace DB.Entity
{
    // ── Full Evaluation Page response ─────────────────────────────────────────
    public class TenderEvaluationPageDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public List<TenderTechnicalEvalRowDto> TechnicalRows { get; set; } = new();
        public List<TenderFinancialEvalRowDto> FinancialRows { get; set; } = new();
        public TenderRecommendationPageDto Recommendation { get; set; } = new();
    }

    // ── Technical Evaluation table row ────────────────────────────────────────
    public class TenderTechnicalEvalRowDto
    {
        public int VendorId { get; set; }
        public string TendererCode { get; set; } = string.Empty;     // e.g. "1/3"
        public string TenderOpeningStatus { get; set; } = "Pending";
        public string TechnicalEvaluationStatus { get; set; } = "Pending";
        public string Result { get; set; } = "Pending";
        public int Ranking { get; set; }
    }

    // ── Technical Evaluation popup (GET) ─────────────────────────────────────
    public class TenderTechnicalEvalPopupDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public string TendererCode { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public List<TechnicalCriterionDto> Criteria { get; set; } = new();
        public int TotalScore { get; set; }
        public int PassingMarks { get; set; }
        public string Result { get; set; } = "Pending";
    }

    public class TechnicalCriterionDto
    {
        public int SpecificationId { get; set; }
        public string Specification { get; set; } = string.Empty;
        public int Weightage { get; set; }
        public int Score { get; set; }        // 0-5 (dropdown value)
        public int Total { get; set; }        // Weightage * Score / 5
        public string? Remarks { get; set; }
    }

    // ── Save technical scores from popup (POST) ───────────────────────────────
    public class SaveTechnicalScoreDto
    {
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public List<TechnicalCriterionDto> Scores { get; set; } = new();
    }

    // ── Financial Evaluation table row (auto-computed from VendorFinancial) ───
    public class TenderFinancialEvalRowDto
    {
        public int VendorId { get; set; }
        public string TendererCode { get; set; } = string.Empty;
        public decimal CapitalLiquidation { get; set; }
        public decimal AssetBalance { get; set; }
        public decimal FinalAmount { get; set; }        // MIN(CapitalLiquidation, AssetBalance)
        public decimal MinCapitalRequired { get; set; } // TenderApplication.MinCapitalAmount
        public string Result { get; set; } = "Pending"; // Passed / Failed
    }

    // ── Recommendation section ────────────────────────────────────────────────
    public class TenderRecommendationPageDto
    {
        public int TenderId { get; set; }
        public int? RecommendedVendorId { get; set; }
        public string? Reason { get; set; }
        public List<TenderRecommRowDto> Rows { get; set; } = new();
        public List<TendererOptionDto> VendorOptions { get; set; } = new();
    }

    public class TenderRecommRowDto
    {
        public int VendorId { get; set; }
        public string TendererCode { get; set; } = string.Empty;
        public string TenderOpeningStatus { get; set; } = "Pending";
        public string TechnicalEvaluationStatus { get; set; } = "Pending";
        public string FinancialEvaluationStatus { get; set; } = "Pending";
        public decimal VendorOfferPrice { get; set; }
        public decimal FpmsEstimationPrice { get; set; }
        public decimal DifferencePercent { get; set; }
    }

    public class TendererOptionDto
    {
        public int VendorId { get; set; }
        public string TendererCode { get; set; } = string.Empty;
    }
}
