using DB.Entity;

namespace DB.Repositories.Interfaces
{
    public interface IBiddingRepository
    {
        // ── Vendor portal ──────────────────────────────────────────────────────
        Task<List<BiddingListDto>> GetActiveBiddingListAsync(int vendorId);
        Task<BiddingDetailDto?> GetBiddingDetailAsync(int tenderId, int vendorId);
        Task SubmitBiddingAsync(SubmitBiddingDto dto);
        Task<BiddingAwardDetailDto?> GetBiddingAwardDetailAsync(int tenderId, int vendorId);

        // ── Admin — manage bidding assets ──────────────────────────────────────
        Task<List<BiddingAssetDto>> GetBiddingAssetsAsync(int tenderId);
        Task SaveBiddingAssetAsync(SaveBiddingAssetDto dto);
        Task DeleteBiddingAssetAsync(int assetId);

        // ── Vendor acknowledgement ─────────────────────────────────────────────
        Task SubmitBidderAcknowledgementAsync(SubmitBidderAcknowledgementDto dto);

        // ── Tender Opening — Verify + Progress ────────────────────────────────
        Task VerifyTenderOpeningAsync(VerifyTenderOpeningDto dto, int userId);
        Task<TenderOpeningProgressDto?> GetTenderOpeningProgressAsync(int tenderId);
    }
}
