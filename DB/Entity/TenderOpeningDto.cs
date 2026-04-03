using System;
using System.Collections.Generic;

namespace DB.Entity
{
    // ── Page 1: Search list ────────────────────────────────────────────────────
    public class TenderOpeningListDto
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TenderStatus { get; set; }
    }

    // ── Page 2: Detail (after clicking Reference No) ───────────────────────────
    public class TenderOpeningDetailDto
    {
        public int Id { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // ── Page 3: Tender Opening page (after Proceed) ───────────────────────────
    public class TenderOpeningPageDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public TenderOpeningSummaryRowDto? Summary { get; set; }
        public List<TenderOpeningVendorDto> Vendors { get; set; } = new();
    }

    public class TenderOpeningSummaryRowDto
    {
        public DateTime? ClosingDate { get; set; }
        public string? ClosingTime { get; set; }      // e.g. "12:00 PM"
        public string? Type { get; set; }             // TenderCategory name
        public string? CategoryCode { get; set; }     // e.g. "010101 AND 010304"
        public decimal? EstimationCost { get; set; }
        public string? Validity { get; set; }         // e.g. "90 DAYS"
    }

    public class TenderOpeningVendorDto
    {
        public int Bil { get; set; }
        public string? VendorName { get; set; }
        public string? BumiStatus { get; set; }       // "Bumiputera" / "Non Bumiputera"
        public DateTime? RegistrationExpiry { get; set; }
        public decimal? OfferedPrice { get; set; }
    }
}
