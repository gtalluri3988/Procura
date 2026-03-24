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
        public async Task<IActionResult> GetTenderApplicationBySearch(TenderAppSearch search)
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
    }
}
