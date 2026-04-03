using Api.Controllers;
using BusinessLogic.Interfaces;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace Procura.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BiddingController : AuthorizedCSABaseAPIController
    {
        private readonly IBiddingService _biddingService;
        private readonly ICurrentUserService _currentUserService;

        public BiddingController(IBiddingService biddingService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<BiddingController> logger)
            : base(userService, logger)
        {
            _biddingService = biddingService;
            _currentUserService = currentUserService;
        }

        // ── Vendor Portal ──────────────────────────────────────────────────────

        /// <summary>Get list of open tenders available for bidding (vendor view)</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetActiveBiddingList(int vendorId)
        {
            var result = await _biddingService.GetActiveBiddingListAsync(vendorId);
            return Ok(result);
        }

        /// <summary>Get full Bidding Details page for a tender (vendor view)</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBiddingDetail(int tenderId, int vendorId)
        {
            var result = await _biddingService.GetBiddingDetailAsync(tenderId, vendorId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Submit bid prices for all assets in a tender</summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitBidding([FromBody] SubmitBiddingDto dto)
        {
            await _biddingService.SubmitBiddingAsync(dto);
            return Ok("Bidding submitted successfully");
        }

        /// <summary>Get Award Details tab (vendor sees if they won)</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBiddingAwardDetail(int tenderId, int vendorId)
        {
            var result = await _biddingService.GetBiddingAwardDetailAsync(tenderId, vendorId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Admin — Bidding Asset Management ──────────────────────────────────

        /// <summary>Get list of assets for a tender (admin view)</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBiddingAssets(int tenderId)
        {
            var result = await _biddingService.GetBiddingAssetsAsync(tenderId);
            return Ok(result);
        }

        /// <summary>Add or edit a bidding asset in a tender</summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveBiddingAsset([FromBody] SaveBiddingAssetDto dto)
        {
            await _biddingService.SaveBiddingAssetAsync(dto);
            return Ok("Bidding asset saved successfully");
        }

        /// <summary>Delete a bidding asset</summary>
        [AllowAnonymous]
        [HttpDelete]
        public async Task<IActionResult> DeleteBiddingAsset(int assetId)
        {
            await _biddingService.DeleteBiddingAssetAsync(assetId);
            return Ok("Bidding asset deleted successfully");
        }

        // ── Vendor Acknowledgement ─────────────────────────────────────────────

        /// <summary>Submit vendor acknowledgement (Accept/Reject) for awarded tender</summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitBidderAcknowledgement([FromBody] SubmitBidderAcknowledgementDto dto)
        {
            await _biddingService.SubmitBidderAcknowledgementAsync(dto);
            return Ok("Acknowledgement submitted successfully");
        }

        // ── Tender Opening — Verify + Progress ────────────────────────────────

        /// <summary>Verify tender opening (committee confirms all bids received)</summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> VerifyTenderOpening([FromBody] VerifyTenderOpeningDto dto)
        {
            int.TryParse(_currentUserService.GetUserId(), out var userId);
            await _biddingService.VerifyTenderOpeningAsync(dto, userId);
            return Ok("Tender opening verified successfully");
        }

        /// <summary>Get Progress tab — verification status + bid summary</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderOpeningProgress(int tenderId)
        {
            var result = await _biddingService.GetTenderOpeningProgressAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
