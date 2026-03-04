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
    public class ResidentAccessHistoryController : AuthorizedCSABaseAPIController
    {
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public readonly IResidentService _residentService;
        public readonly IResidentAccessHistoryService _residentAccessHistoryService;
        public ResidentAccessHistoryController(
            ICurrentUserService currentUserService,
            ICommunityService communityService,
            IResidentService residentService,
            IUserService userService,
            IResidentAccessHistoryService residentAccessHistoryService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService = communityService;
            _residentService = residentService;
            _residentAccessHistoryService = residentAccessHistoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllResidentsAccessHistory(int? communityId)
        {
            
            return Ok(await _residentAccessHistoryService.GetAllResidentAccessHistoryAsync(communityId, IsCSAAdmin()));
        }
        [HttpGet]
        public async Task<IActionResult> GetResidentAccessHistoryByResidentId(int residentId)
        {
            return Ok(await _residentAccessHistoryService.GetResidentAccessHistoryByIdAsync(residentId));
        }
        [HttpPost]
        public async Task<IActionResult> CreateResidentAccessHistory(ResidentAccessHistoryDTO residentAccessModel)
        {
            Random random = new Random();
            var createdResident = await _residentAccessHistoryService.SaveResidentAccessHistoryAsync(residentAccessModel);
            return CreatedAtAction(nameof(GetResidentAccessHistoryByResidentId), new { id = createdResident.Id }, createdResident);
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentsAccessHistoryById(int? AccessId)
        {

            return Ok(await _residentAccessHistoryService.GetResidentAccessHistoryByIdAsync(AccessId));
        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateResidentAccessHistory(int id, ResidentDTO dto)
        //{
        //    await _residentAccessHistoryService.UpdateResidentAsync(id, dto);
        //    return NoContent();
        //}

        [HttpPost]
        public async Task<IActionResult> GetAllResidentAccessHistoryBysearchParams(ResidentAccessHistoryDTO Params)
        {
            return Ok(await _residentAccessHistoryService.SearchResidentAccessHistoryBySearchParamsAsync(Params));
        }

    }

    
}
