using Api.Controllers;
using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace Procura.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AnnouncementController : AuthorizedCSABaseAPIController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly ICurrentUserService _currentUserService;

        public AnnouncementController(IAnnouncementService announcementService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<ContentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _announcementService = announcementService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAnnouncements(AnnouncementType type)
        {
            return Ok(await _announcementService.GetAnnouncementsAsync(type));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAnnouncementById(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null)
                return NotFound();
            return Ok(announcement);
        }
        [HttpGet]
        public async Task<IActionResult> GetAnnouncementsByType(AnnouncementType type)
        {
            return Ok(await _announcementService.GetAnnouncementsAsync(type));
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(AnnouncementDto dto)
        {
            var createdAnnouncement = await _announcementService.AddAnnouncementAsync(dto);
            return CreatedAtAction(nameof(GetAnnouncementById), new { id = createdAnnouncement }, createdAnnouncement);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAnnouncement(AnnouncementDto dto)
        {
            await _announcementService.UpdateAnnouncementAsync(dto);
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            await _announcementService.DeleteAnnouncementAsync(id);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(AnnouncementDto dto)
        {
            var createdId = await _announcementService.AddAnnouncementAsync(dto);
            return CreatedAtAction(nameof(GetAnnouncementById), new { id = createdId }, createdId);
        }
    }
}
