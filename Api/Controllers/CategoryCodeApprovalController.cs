using Api.Controllers;
using BusinessLogic.Interfaces;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Procura.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryCodeApprovalController : AuthorizedCSABaseAPIController
    {
        private readonly ICategoryCodeApprovalService _approvalService;

        public CategoryCodeApprovalController(
            ICategoryCodeApprovalService approvalService,
            IUserService userService,
            ILogger<CategoryCodeApprovalController> logger)
            : base(userService, logger)
        {
            _approvalService = approvalService;
        }

        /// <summary>
        /// Pre-check eligibility (freeze period, change limits) before the vendor opens the form.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ValidateEligibility(int vendorId)
        {
            var result = await _approvalService.ValidateAndCheckEligibilityAsync(vendorId);
            return Ok(result);
        }

        /// <summary>
        /// Vendor submits a new category change request. Creates a Pending record in CategoryCodeApprovals.
        /// Only for SAP-approved vendors; unapproved vendors use the direct POST /api/Vendor/Categories.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitRequest(int vendorId, VendorCategoryRequest request)
        {
            try
            {
                var requestId = await _approvalService.SubmitCategoryChangeRequestAsync(vendorId, request);
                return Ok(new { RequestId = requestId, Message = "Category change request submitted for approval." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// List approval requests. Optionally filter by status (Pending, Approved, Rejected).
        /// Used by FPMSB staff on the Vendor Approval tab (ReviewList.png).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRequests(string? status)
        {
            var requests = await _approvalService.GetApprovalRequestsAsync(status);
            return Ok(requests);
        }

        /// <summary>
        /// Get single approval request with full detail (categories per code master).
        /// Used by FPMSB staff on the approval detail page (CategorycodeApprovalPage.png).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRequestDetail(int requestId)
        {
            var detail = await _approvalService.GetApprovalRequestByIdAsync(requestId);
            if (detail == null)
                return NotFound(new { Error = "Approval request not found." });

            return Ok(detail);
        }

        /// <summary>
        /// Approve a pending category change request.
        /// Inserts categories into VendorCategory and logs the change.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Approve(ApproveCategoryChangeRequest request)
        {
            try
            {
                await _approvalService.ApproveCategoryChangeAsync(request.RequestId);
                return Ok(new { Message = "Category change approved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Reject a pending category change request. Reason is mandatory.
        /// Does NOT insert into VendorCategory or log a change count.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reject(RejectCategoryChangeRequest request)
        {
            try
            {
                await _approvalService.RejectCategoryChangeAsync(request.RequestId, request.Reason);
                return Ok(new { Message = "Category change rejected." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
