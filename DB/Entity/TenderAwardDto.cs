using System;
using System.Collections.Generic;

namespace DB.Entity
{
    // ── Award list (search page) ───────────────────────────────────────────────
    public class TenderAwardListDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public string? RecommendedVendorName { get; set; }
        public string? AwardStatus { get; set; }    // Pending / Awarded
    }

    // ── Minutes of Meeting ─────────────────────────────────────────────────────
    public class TenderAwardMinutesDto
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? MeetingOutcome { get; set; }
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }
    }

    // ── Vendor Appointment Details ─────────────────────────────────────────────
    public class TenderAwardVendorAppointmentDto
    {
        public int? AwardedVendorId { get; set; }
        public string? AwardedVendorName { get; set; }
        public decimal? ProjectValue { get; set; }
        public decimal? YearlyExpenses { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public string? Agreement { get; set; }    // "Yes" or "No"
        public DateTime? AgreementDateSigned { get; set; }
        public string? PONumber { get; set; }
    }

    // ── Insurance Details (auto-computed, read-only on UI) ─────────────────────
    public class TenderAwardInsuranceDto
    {
        // Public Liability
        public string PublicLiabilityFormula { get; set; } = "10% of total contract sum or a minimum of RM2,000,000.00";
        public decimal PublicLiabilityValue { get; set; }
        public DateTime? PublicLiabilityPeriodStart { get; set; }
        public DateTime? PublicLiabilityPeriodEnd { get; set; }

        // Contractor at Risk
        public string ContractorAtRiskFormula { get; set; } = "Full Replacement Value of the Works + 10% for professional fees";
        public decimal ContractorAtRiskValue { get; set; }
        public DateTime? ContractorAtRiskPeriodStart { get; set; }
        public DateTime? ContractorAtRiskPeriodEnd { get; set; }

        // Worksman Compensation
        public string WorksmanCompensationFormula { get; set; } = "As per Statutory Requirements (Common Law)";
        public decimal WorksmanCompensationValue { get; set; }
        public DateTime? WorksmanCompensationPeriodStart { get; set; }
        public DateTime? WorksmanCompensationPeriodEnd { get; set; }

        // LAD
        public string LADFormula { get; set; } = "RM500.00 per day of delay up to a maximum of 10% of contract value";
        public decimal LADValue { get; set; }   // Fixed RM500/day
    }

    // ── Vendor options for award dropdown (only the recommended vendor) ───────
    public class TenderAwardVendorOptionDto
    {
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
    }

    // ── Full Award Page (GET + POST) ───────────────────────────────────────────
    public class TenderAwardPageDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }

        public List<TenderAwardMinutesDto> MinutesOfMeetings { get; set; } = new();
        public TenderAwardVendorAppointmentDto VendorAppointment { get; set; } = new();
        public TenderAwardInsuranceDto Insurance { get; set; } = new();

        // Vendor options populated from recommendation/evaluation results
        public List<TenderAwardVendorOptionDto> VendorOptions { get; set; } = new();
    }

    // ── Save Minutes of Meeting (Add popup) ────────────────────────────────────
    public class SaveTenderAwardMinutesDto
    {
        public int Id { get; set; }         // 0 = new
        public int TenderId { get; set; }
        public DateTime MeetingDate { get; set; }
        public string? MeetingOutcome { get; set; }
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }
    }

    // ── Save Award Page (Save button) ──────────────────────────────────────────
    public class SaveTenderAwardDto
    {
        public int TenderId { get; set; }
        public TenderAwardVendorAppointmentDto VendorAppointment { get; set; } = new();
    }
}
