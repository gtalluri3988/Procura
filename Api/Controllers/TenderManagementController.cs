using Api.Controllers;
using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Procura.Models;
using YourNamespace.Services;

namespace Procura.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TenderManagementController : AuthorizedCSABaseAPIController
    {
        private readonly IContentService _contentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVendorService _vendorService;
        private readonly ISAPServices _sapService;
        private readonly ITenderService _tenderService;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();


        public TenderManagementController(IContentService contentService,
            ICurrentUserService currentUserService,
            IUserService userService, IVendorService vendorService, ISAPServices sapServices,ITenderService tenderService,
            ILogger<ContentController> logger, IConfiguration configuration)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _contentService = contentService;
            _vendorService = vendorService;
            _tenderService = tenderService;
            _sapService = sapServices;
            _configuration = configuration;
        }

       
        [HttpPost]
        public async Task<IActionResult> SaveTenderApplication(TenderApplicationDto dto)
        {
           var tender=await _tenderService.AddTenderApplicationAsync(dto);
            return Ok(tender);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpdateTenderApplication(TenderApplicationDto dto)
        {
            var tender = await _tenderService.UpdateTenderApplicationAsync(dto);
            return Ok(tender);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> DeleteTenderApplication(int tendrId)
        {
            await _tenderService.DeleteTenderApplicationAsync(tendrId);
            return Ok("Tender application removed successfully");
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTenderApplications()
        {
            var result = await _tenderService.GetAllTenderApplicationsAsync();
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderApplicationById(int id)
        {
            var result = await _tenderService.GetTenderApplicationByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderApplicationBySearch([FromQuery] TenderAppSearch search)
        {
            var result = await _tenderService.GetAllTenderApplicationsAsync(search.applicationLevelId, search.tenderCategoryId, 
                search.jobCategoryId, search.statusId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetReviewersorApprovers(int ApplicationLevelId, int DesignationId,int StateId)
        {
            var result = await _tenderService.GetTendorReviewers(ApplicationLevelId, DesignationId, StateId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTenderAdvertisementPage([FromBody] TenderAdvertisementPageDto dto)
        {
            await _tenderService.SaveTenderAdvertisementPageAsync(dto);
            return Ok("Tender advertisement page saved successfully");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderAdvertisementPage(int tenderApplicationId)
        {
            var result = await _tenderService.GetTenderAdvertisementPageByTenderApplicationIdAsync(tenderApplicationId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMasterEvaluationCriteria(int jobCategoryId)
        {
            var criteria = await _tenderService.GetMasterEvaluationCriteriaAsync(jobCategoryId);
            return Ok(criteria);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAdvertisedTenders()
        {
            var result = await _tenderService.GetAdvertisedTendersAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AdvertiseTender(int tenderId)
        {
            await _tenderService.AdvertiseTenderAsync(tenderId);
            return Ok("Tender advertised successfully");
        }

        // ── Tender Opening — List (Page 1) ─────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderOpeningList(string? referenceId, string? projectName)
        {
            var result = await _tenderService.GetTenderOpeningListAsync(referenceId, projectName);
            return Ok(result);
        }

        // ── Tender Opening — Detail (Page 2, click on Reference No) ───────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderOpeningDetail(int tenderId)
        {
            var result = await _tenderService.GetTenderOpeningDetailAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Tender Opening — Full Opening Page (Page 3, after Proceed) ────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderOpeningPage(int tenderId)
        {
            var result = await _tenderService.GetTenderOpeningPageAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SearchCommitteeUsers(string name, string committeeType)
        {
            if (string.IsNullOrWhiteSpace(committeeType) ||
                (!committeeType.Equals("opening", StringComparison.OrdinalIgnoreCase) &&
                 !committeeType.Equals("evaluation", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("committeeType must be 'opening' or 'evaluation'");
            }

            var result = await _tenderService.SearchCommitteeUsersAsync(name, committeeType);
            return Ok(result);
        }

        // ── Tender Evaluation — Full Page ─────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderEvaluationPage(int tenderId)
        {
            var result = await _tenderService.GetTenderEvaluationPageAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Tender Evaluation — Technical Popup ───────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTechnicalEvaluationPopup(int tenderId, int vendorId)
        {
            var result = await _tenderService.GetTechnicalEvaluationPopupAsync(tenderId, vendorId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Tender Evaluation — Save Technical Scores ────────────────────────
        [HttpPost]
        public async Task<IActionResult> SaveTechnicalScore([FromBody] SaveTechnicalScoreDto dto)
        {
            await _tenderService.SaveTechnicalScoreAsync(dto);
            return Ok("Technical scores saved successfully");
        }

        // ── Tender Evaluation — Save Recommendation ───────────────────────────
        [HttpPost]
        public async Task<IActionResult> SaveTenderRecommendation([FromBody] TenderRecommendationPageDto dto)
        {
            await _tenderService.SaveTenderRecommendationAsync(dto);
            return Ok("Recommendation saved successfully");
        }

        // ── Tender Award — List ────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderAwardList(string? referenceId, string? projectName)
        {
            var result = await _tenderService.GetTenderAwardListAsync(referenceId, projectName);
            return Ok(result);
        }

        // ── Tender Award — Full Page ───────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderAwardPage(int tenderId)
        {
            var result = await _tenderService.GetTenderAwardPageAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Tender Award — Save Vendor Appointment ────────────────────────────
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveTenderAward([FromBody] SaveTenderAwardDto dto)
        {
            await _tenderService.SaveTenderAwardAsync(dto);
            return Ok("Tender award saved successfully");
        }

        // ── Tender Award — Save Minutes of Meeting (Add/Edit popup) ──────────
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveTenderAwardMinutes([FromBody] SaveTenderAwardMinutesDto dto)
        {
            var result = await _tenderService.SaveTenderAwardMinutesAsync(dto);
            return Ok(result);
        }

        // ── Tender Award — Delete Minutes of Meeting ──────────────────────────
        [AllowAnonymous]
        [HttpDelete]
        public async Task<IActionResult> DeleteTenderAwardMinutes(int minutesId)
        {
            await _tenderService.DeleteTenderAwardMinutesAsync(minutesId);
            return Ok("Minutes of meeting deleted successfully");
        }

        // ── Vendor Performance — Get Page ──────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVendorPerformancePage(int tenderId)
        {
            var result = await _tenderService.GetVendorPerformancePageAsync(tenderId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ── Vendor Performance — Save ──────────────────────────────────────────
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveVendorPerformance([FromBody] SaveVendorPerformanceDto dto)
        {
            int.TryParse(_currentUserService.GetUserId(), out var userId);
            await _tenderService.SaveVendorPerformanceAsync(dto, userId);
            return Ok("Vendor performance saved successfully");
        }

        // ════════════════════════════════════════════════════════════════════
        //  APPROVAL WORKFLOW
        // ════════════════════════════════════════════════════════════════════

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetApprovalWorkflow(int tenderId)
        {
            var result = await _tenderService.GetApprovalWorkflowAsync(tenderId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InitializeWorkflow(int tenderId, decimal? estimatedPrices, int siteLevelId, int siteOfficeId)
        {
            await _tenderService.InitializeWorkflowAsync(tenderId, estimatedPrices, siteLevelId, siteOfficeId);
            return Ok("Workflow initialized");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ApproveRejectWorkflow([FromBody] ApproveRejectRequest request)
        {
            var userIdClaim = HttpContext.User.FindFirst("userid")?.Value ?? "0";
            var userId = Convert.ToInt32(userIdClaim);
            await _tenderService.ApproveRejectWorkflowAsync(
                request.TenderId, request.Stage, request.Level, request.Status, request.Remarks, userId);
            return Ok("Workflow updated");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangeWorkflowApprover([FromBody] ChangeApproverRequest request)
        {
            await _tenderService.ChangeWorkflowApproverAsync(
                request.TenderId, request.Stage, request.Level, request.NewUserId, request.ChangeRemarks);
            return Ok("Approver changed");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InitializeIssuanceWorkflow(int tenderId, int siteLevelId, int siteOfficeId)
        {
            await _tenderService.InitializeIssuanceWorkflowAsync(tenderId, siteLevelId, siteOfficeId);
            return Ok("Issuance workflow initialized");
        }
    }
}

// Request models
namespace Procura.Models
{
    public class ApproveRejectRequest
    {
        public int TenderId { get; set; }
        public string Stage { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }
    }

    public class ChangeApproverRequest
    {
        public int TenderId { get; set; }
        public string Stage { get; set; }
        public int Level { get; set; }
        public int NewUserId { get; set; }
        public string? ChangeRemarks { get; set; }
    }
}
