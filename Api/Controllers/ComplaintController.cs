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
    public class ComplaintController : AuthorizedCSABaseAPIController
    {
        private readonly IComplaintService _complaintService;
        private readonly ICurrentUserService _currentUserService;

        public ComplaintController(
            IComplaintService complaintService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _complaintService = complaintService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            try
            {
                return Ok(await _complaintService.GetAllComplaintAsync());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllComplaintsResident()
        {
            try
            {
                return Ok(await _complaintService.GetAllComplaintForResidentAsync());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComplaintsByCommunity(int communityId)
        {
            try
            {
                return Ok(await _complaintService.GetAllComplaintsByCommunity(communityId));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplaintById(int id)
        {
            var visitor = await _complaintService.GetComplaintByIdAsync(id);
            if (visitor == null)
                return NotFound();
            return Ok(visitor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComplaint(ComplaintDTO dto)
        {
            string uploadedBy = IsCSAAdmin() || IsResidentAdmin() ? "Admin" : "Resident";

            if (dto.ComplaintPhotos?.Count > 0)
            {
                foreach (var photo in dto.ComplaintPhotos)
                {
                    photo.ImageUploadedBy = uploadedBy;
                }
            }
            var createdComplaint = await _complaintService.CreateComplaintAsync(dto);
            return CreatedAtAction(nameof(GetComplaintById), new { id = createdComplaint.Id }, createdComplaint);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComplaint(int id,  ComplaintDTO dto)
        {
            List<IFormFile> photos = null;
            string uploadedBy = IsCSAAdmin() || IsResidentAdmin() ? "Admin" : "Resident";

            if (dto.ComplaintPhotos?.Count > 0)
            {
                foreach (var photo in dto.ComplaintPhotos)
                {
                    photo.ImageUploadedBy = uploadedBy;
                }
            }
            await _complaintService.UpdateComplaintAsync(id, dto, photos);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            await _complaintService.DeleteComplaintAsync(id);
            return NoContent();
        }


        [HttpPost]
        public async Task<IActionResult> GetAllComplaintsBysearchParams(ComplaintDTO Params)
        {
            return Ok(await _complaintService.SearchComplaintBySearchParamsAsync(Params));
        }

        [HttpGet]
        public async Task<IActionResult> SubmitComplaint(int complaintId)
        {
            await _complaintService.SubmitComplaintAsync(complaintId);
            return NoContent();
        }

    }
}
