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
    public class CommunityController : AuthorizedCSABaseAPIController
    {
        public class Community
        {
            public List<CommunityDTO> CommunityResult {get; set;}
        }
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public CommunityController(
            ICurrentUserService currentUserService,
            ICommunityService communityService ,IUserService userService,
            ILogger<AuthorizedCSABaseAPIController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService= communityService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCommunityTypes()
        {
            return Ok(await _communityService.GetCommunityTypeList());
        }
        [HttpPost]
        public async Task<IActionResult> SaveCommunity([FromBody] CommunityObject community)
        {
            return Ok(await _communityService.GetCommunityTypeList());
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCommunities()
        {
            return Ok(new CommunityDTO { CommunityResult = await _communityService.GetAllCommunitiesAsync() });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommunityById(int id)
        {
            var Community = await _communityService.GetCommunityByIdAsync(id);
            //if (Community == null)
            //    return NotFound();
            return Ok(Community);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(CommunityDTO dto)
        {
            dto.CommunityId = await _communityService.GetNextNumberAsync();
            var createdCommunity = await _communityService.CreateCommunityAsync(dto);
            return CreatedAtAction(nameof(GetCommunityById), new { id = createdCommunity.Id }, createdCommunity);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCommunity(int id, [FromBody] CommunityDTO dto)
        {
            await _communityService.UpdateCommunityAsync(id, dto);
            return Ok(true);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteCommunity(int id)
        {
            try
            {
                await _communityService.DeleteCommunityAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCommunitiesWithStates()
        {
            return Ok(new CommunityDTO { CommunityResult = await _communityService.GetAllCommunitiesWithStatesAsync() });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCommunitiesWithResidentCount()
        {
            return Ok(await _communityService.GetCommunitiesWithResidentCountAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCityByStateId(int stateId)
        {
            return Ok(await _communityService.GetCityByStateAsync(stateId));
        }
    }
}
