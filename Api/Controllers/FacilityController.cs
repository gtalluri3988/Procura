using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.EFModel;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FacilityController : AuthorizedCSABaseAPIController
    {
        private readonly IFacilityService _facilityService;
        private readonly ICurrentUserService _currentUserService;

        public FacilityController(IFacilityService facilityService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _facilityService = facilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFacility()
        {
            return Ok(await _facilityService.GetAllFacilityAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetFacilityById(int id)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(id);
            if (facility == null)
                return NotFound();
            return Ok(facility);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFacility(FacilityDTO dto)
        {
            var createdVisitor = await _facilityService.CreateFacilityAsync(dto);
            return CreatedAtAction(nameof(GetFacilityById), new { id = createdVisitor.Id }, createdVisitor);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateFacility(int id, FacilityDTO dto)
        {
            await _facilityService.UpdateFacilityAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            await _facilityService.DeleteFacilityAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> SearchFacility(int communityId, int facilityTypeId)
        {
            return Ok(await _facilityService.SearchFacilityAsync(communityId, facilityTypeId));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFacilitiesByCommunityId(int communityId)
        {
            try
            {
                return Ok(await _facilityService.GetAllFacilitiesByCommunityAsync(communityId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFacilitiesByfacilityType(int communityId, int facilityTypeId)
        {
            return Ok(await _facilityService.GetAllFacilitiesByfacilityTypeAsync(communityId, facilityTypeId));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFacilityByAvilableLotQty(int id)
        {
            try
            {
                var facility = await _facilityService.GetAllFacilityByAvilableLotQtyIdAsync(id);
                if (facility == null)
                    return NotFound();
                return Ok(facility);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return only the message
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLotAvilabilityByStartMonth(string startMonth,int facilityId)
        {
            try
            {
                var lotAvilability = await _facilityService.GetLotAvilabilityByMonth(startMonth,facilityId);
                if (lotAvilability == null)
                    return NotFound();
                return Ok(new { lotAvilability = lotAvilability });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Return only the message
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFacilityById(int facilityId)
        {
            await _facilityService.DeleteFacility(facilityId);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFacilityHistoryByCommunity(int communityId)
        {
            try
            {
                return Ok(await _facilityService.GetAllFacilityHistoryByCommunityAsync(communityId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAllFacilityHistoryByResident(int communityId)
        {
            try
            {
                return Ok(await _facilityService.GetAllFacilityHistoryByResidentAsync(communityId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
