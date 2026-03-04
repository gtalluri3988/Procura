using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Models;
using YourNamespace.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Services;
using DB.Entity;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ReportController : AuthorizedCSABaseAPIController
    {
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public readonly IResidentService _residentService;
        public readonly IResidentAccessHistoryService _residentAccessHistoryService;
        public readonly IReportService _reportService;
        public ReportController(
            ICurrentUserService currentUserService,
            ICommunityService communityService,
            IResidentService residentService,
            IUserService userService,
            IResidentAccessHistoryService residentAccessHistoryService,IReportService reportService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService = communityService;
            _residentService = residentService;
            _residentAccessHistoryService = residentAccessHistoryService;
            _reportService = reportService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCollectionSummaryReport(int? communityId)
        {          
            return Ok(await _reportService.GetCollectionSummaryReportAsync(communityId));
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingRevenueReport(int? communityId)
        {
            return Ok(await _reportService.GetFacilityUsageReportAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetOutstandingPaymentReport(int? communityId)
        {
            return Ok(await _reportService.GetMaintenanceReportSummaryAsync());
        }


        [HttpGet]
        public async Task<IActionResult> GetVisitorSummaryReport(int? communityId,int year)
        {
            return Ok(await _reportService.GetMonthlyVisitorReportAsync(communityId, year));
        }

        [HttpGet]
        public async Task<IActionResult> GetCarParkCollectionReport(int? communityId)
        {
            return Ok(await _reportService.GetCarParkCollectionReportAsync());
        }


    }


}

