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
    public class ContentController : AuthorizedCSABaseAPIController
    {
        private readonly IContentService _contentService;
        private readonly ICurrentUserService _currentUserService;

        public ContentController(IContentService contentService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<ContentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContent()
        {
            return Ok(await _contentService.GetAllContentsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetContentById(int id)
        {
            var Content = await _contentService.GetContentByIdAsync(id);
            if (Content == null)
                return NotFound();
            return Ok(Content);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent(ContentManagementDTO dto)
        {
            var createdContent = await _contentService.CreateContentAsync(dto);
            return CreatedAtAction(nameof(GetContentById), new { id = createdContent.Id }, createdContent);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateContent(int id, ContentManagementDTO dto)
        {
            await _contentService.UpdateContentAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            await _contentService.DeleteContentAsync(id);
            return NoContent();
        }
    }
}
