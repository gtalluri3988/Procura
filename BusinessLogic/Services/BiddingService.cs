using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories.Interfaces;
using DB.Repositories;

namespace BusinessLogic.Services
{
    public class BiddingService : IBiddingService
    {
        private readonly IBiddingRepository _biddingRepository;

        public BiddingService(IBiddingRepository biddingRepository)
        {
            _biddingRepository = biddingRepository;
        }

        public async Task<List<BiddingListDto>> GetActiveBiddingListAsync(int vendorId)
            => await _biddingRepository.GetActiveBiddingListAsync(vendorId);

        public async Task<BiddingDetailDto?> GetBiddingDetailAsync(int tenderId, int vendorId)
            => await _biddingRepository.GetBiddingDetailAsync(tenderId, vendorId);

        public async Task SubmitBiddingAsync(SubmitBiddingDto dto)
            => await _biddingRepository.SubmitBiddingAsync(dto);

        public async Task<BiddingAwardDetailDto?> GetBiddingAwardDetailAsync(int tenderId, int vendorId)
            => await _biddingRepository.GetBiddingAwardDetailAsync(tenderId, vendorId);

        public async Task<List<BiddingAssetDto>> GetBiddingAssetsAsync(int tenderId)
            => await _biddingRepository.GetBiddingAssetsAsync(tenderId);

        public async Task SaveBiddingAssetAsync(SaveBiddingAssetDto dto)
            => await _biddingRepository.SaveBiddingAssetAsync(dto);

        public async Task DeleteBiddingAssetAsync(int assetId)
            => await _biddingRepository.DeleteBiddingAssetAsync(assetId);

        public async Task SubmitBidderAcknowledgementAsync(SubmitBidderAcknowledgementDto dto)
            => await _biddingRepository.SubmitBidderAcknowledgementAsync(dto);

        public async Task VerifyTenderOpeningAsync(VerifyTenderOpeningDto dto, int userId)
            => await _biddingRepository.VerifyTenderOpeningAsync(dto, userId);

        public async Task<TenderOpeningProgressDto?> GetTenderOpeningProgressAsync(int tenderId)
            => await _biddingRepository.GetTenderOpeningProgressAsync(tenderId);
    }
}
