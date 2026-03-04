using BusinessLogic.Interfaces;
using BusinessLogic.Services;
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
    public class NotificationController : AuthorizedCSABaseAPIController
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;

        public NotificationController(INotificationService notificationService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<NotificationController> logger)
            : base(userService, logger)
        {
            _notificationService = notificationService;
            _currentUserService = currentUserService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            return Ok(await _notificationService.GetAllNotificationsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var Event = await _notificationService.GetNotoficationByIdAsync(id);
            if (Event == null)
                return NotFound();
            return Ok(Event);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotifications(NotificationDTO dto)
        {
            var Event = await _notificationService.SaveNotificationAsync(dto);
            return CreatedAtAction(nameof(GetNotificationById), new { id = Event.Id }, Event);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNotofication(int id, NotificationDTO dto)
        {
            await _notificationService.UpdateNotificationsAsync(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationResidentById(int residentId)
        {
            var Event = await _notificationService.GetNotificationByResidentIdAsync(residentId);
            if (Event == null)
                return NotFound();
            return Ok(Event);
        }

    }
}
