using System;
using System.Collections.Generic;

namespace DB.Entity
{
    // ── Bidding list (vendor sees available open tenders) ─────────────────────
    public class BiddingListDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? BiddingTitle { get; set; }
        public string? TenderCategory { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string? ClosingTime { get; set; }
        public string? Status { get; set; }           // Open / Closed
    }

    // ── Asset row in Bidding Details page ─────────────────────────────────────
    public class BiddingAssetDto
    {
        public int Id { get; set; }
        public int SequenceNo { get; set; }
        public string? ProjectState { get; set; }
        public string? AssetDetails { get; set; }
        public string? AssetRefNo { get; set; }
        public decimal StartingPrice { get; set; }
        public int? YearPurchased { get; set; }
        public decimal BidPrice { get; set; }         // vendor's entered bid (0 if not yet bid)
    }

    // ── Bidding Details page (vendor view) ─────────────────────────────────────
    public class BiddingDetailDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ApplicationLevel { get; set; }
        public string? BiddingTitle { get; set; }
        public string? JobCategory { get; set; }
        public string? TenderCategory { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? Remarks { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string? ClosingTime { get; set; }      // e.g. "12:00PM"
        public bool AlreadySubmitted { get; set; }
        public List<BiddingAssetDto> Assets { get; set; } = new();
    }

    // ── Award Details tab (vendor sees their award + submits acknowledgement) ──
    public class BiddingAwardDetailDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? BidderName { get; set; }         // Awarded vendor company name
        public string? BiddingTitle { get; set; }       // Project name
        public bool IsAwarded { get; set; }
        public DateTime? AwardDate { get; set; }        // TenderAward.AgreementDateSigned

        // Acknowledgement (filled by vendor)
        public string? Acknowledgement { get; set; }    // "Accept" / "Reject" / null if not yet submitted
        public string? StampDutyPath { get; set; }
        public string? StampDutyFileName { get; set; }
        public DateTime? AcknowledgementDateTime { get; set; }
        public bool AlreadyAcknowledged { get; set; }

        // Dropdown options for UI
        public List<string> AcknowledgementOptions { get; set; } = new() { "Accept", "Reject" };
    }

    // ── Submit vendor acknowledgement (Award Details → Submit button) ─────────
    public class SubmitBidderAcknowledgementDto
    {
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public string Acknowledgement { get; set; } = string.Empty;  // "Accept" or "Reject"
        public string? StampDutyPath { get; set; }
        public string? StampDutyFileName { get; set; }
    }

    // ── Submit bidding payload (vendor submits bid prices per asset) ──────────
    public class SubmitBiddingDto
    {
        public int TenderId { get; set; }
        public int VendorId { get; set; }
        public List<BidItemDto> BidItems { get; set; } = new();
    }

    public class BidItemDto
    {
        public int BiddingAssetId { get; set; }
        public decimal BidPrice { get; set; }
    }

    // ── Save/manage bidding assets (admin side) ───────────────────────────────
    public class SaveBiddingAssetDto
    {
        public int Id { get; set; }       // 0 = new
        public int TenderId { get; set; }
        public int SequenceNo { get; set; }
        public string? ProjectState { get; set; }
        public string? AssetDetails { get; set; }
        public string? AssetRefNo { get; set; }
        public decimal StartingPrice { get; set; }
        public int? YearPurchased { get; set; }
    }

    // ── Tender Opening: Verify payload ────────────────────────────────────────
    public class VerifyTenderOpeningDto
    {
        public int TenderId { get; set; }
        public string? Remarks { get; set; }
    }

    // ── Tender Opening: Progress tab ──────────────────────────────────────────
    public class TenderOpeningProgressDto
    {
        public int TenderId { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public string OpeningStatus { get; set; } = "Pending";  // Pending / Verified
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedByName { get; set; }
        public string? Remarks { get; set; }
        public int TotalBidsReceived { get; set; }
        public List<TenderOpeningBidProgressDto> BidProgress { get; set; } = new();
    }

    public class TenderOpeningBidProgressDto
    {
        public int Bil { get; set; }
        public string? VendorName { get; set; }
        public decimal? OfferedPrice { get; set; }
        public string OpeningStatus { get; set; } = "Pending";
    }
}
