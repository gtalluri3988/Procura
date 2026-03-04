using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Models;
using YourNamespace.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Services;
using DB.Entity;
using BusinessLogic.Models.Users;
using System.Text.RegularExpressions;
using DB.Repositories.Interfaces;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ResidentFacilityBookingController : AuthorizedCSABaseAPIController
    {
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public readonly IResidentService _residentService;
        public readonly IFacilityBookingService _facilityBookingService;
        public ResidentFacilityBookingController(
            ICurrentUserService currentUserService,
            ICommunityService communityService,
            IResidentService residentService,
            IUserService userService,
            IFacilityBookingService facilityBookingService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService = communityService;
            _residentService = residentService;
            _facilityBookingService = facilityBookingService;
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateResidentFacilityBooking(FacilityBookingDTO dto)
        {
            try
            {
                Random random = new Random();
                var createdResident = await _facilityBookingService.CreateFacilityBookingAsync(dto);
                return CreatedAtAction(nameof(CreateResidentFacilityBookingById), new { id = createdResident.Id }, createdResident);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateResidentFacilityBookingById(FacilityBookingDTO dto)
        {
            try
            {
                Random random = new Random();
                var createdResident = await _facilityBookingService.CreateFacilityBookingAsync(dto);
                return CreatedAtAction(nameof(GetResidentFacilityBookingById), new { id = createdResident.Id }, createdResident);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetResidentFacilityBookingById(int BookingId)
        {
            return Ok(await _facilityBookingService.GetResidentFacilityBookingById(BookingId));
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentFacilityBookingByFacilityId(int facilityId,int residentId)
        {
            return Ok(await _facilityBookingService.GetResidentFacilityBookingByFavilityId(facilityId, residentId));
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentFacilityBookingByFacility(int facilityId)
        {
            return Ok(await _facilityBookingService.GetResidentFacilityBookingByFavilityId(facilityId));
        }

        [HttpGet]
        public async Task<IActionResult> GetResidentFacilityBookingByBookingId(int bookingId)
        {
            return Ok(await _facilityBookingService.GetResidentFacilityBookingByBookingId(bookingId));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateFacilityBooking(int id, FacilityBookingDTO dto)
        {
            await _facilityBookingService.UpdateFacilityBookingAsync(id, dto);
            return NoContent();
        }
    }
}

