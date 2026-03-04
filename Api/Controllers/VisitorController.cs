using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VisitorController : AuthorizedCSABaseAPIController
    {
        private readonly IVisitorService _visitorService;
        public VisitorController(IVisitorService visitorService,
            IUserService userService,
            ILogger<ResidentController> logger
            ) : base(userService, logger)
        {
            _visitorService = visitorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisitors()
        {
            try
            {
                var visitorList = await _visitorService.GetAllVisitorsAsync();
                return Ok(visitorList);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisitorsByCommunityId(int communityId)
        {
            try
            {
                
                return Ok(await _visitorService.GetAllVisitorsAsync(communityId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitorById(int id)
        {
            var visitor = await _visitorService.GetVisitorsByCommunityResidentIdAsync(id);
            if (visitor == null)
                return NotFound();
            return Ok(visitor);
        }

        [HttpGet]
        public async Task<IActionResult> GetVisitorByCommunityResidentId(int visitorId)
        {
            var visitor = await _visitorService.GetVisitorsByCommunityResidentIdAsync(visitorId);
            if (visitor == null)
                return NotFound();
            return Ok(visitor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisitor(VisitorAccessDetailsDTO dto)
        {
            try
            {
                if (IsResidentAdmin() || IsCSAAdmin())
                {
                   
                    dto.IsAddByAdmin = true;
                }
                var createdVisitor = await _visitorService.CreateVisitorAsync(dto);
                return CreatedAtAction(nameof(GetVisitorById), new { id = createdVisitor.Id }, createdVisitor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVisitor(int id, VisitorAccessDetailsDTO dto)
        {
            await _visitorService.UpdateVisitorAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            await _visitorService.DeleteVisitorAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllVisitorsBysearchParams(VisitorAccessDetailsDTO Params)
        {

            return Ok(await _visitorService.GetAllVisitorsBysearchParams(Params));
        }

        [HttpPost]
        public async Task<IActionResult> SaveVisitor(VisitorAccessDetailsDTO dto)
        {
            try
            {
                dto.RegisterBy= IsResidentAdmin();
                if(IsResidentAdmin() || IsCSAAdmin())
                {
                    //dto.VisitorAccessTypeId = 2;
                    dto.IsAddByAdmin = true;
                }
                var createdVisitor = await _visitorService.SaveVisitorAsync(dto);
                return CreatedAtAction(nameof(GetVisitorById), new { id = createdVisitor.Id }, createdVisitor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveVisitorMobile(VisitorAccessDetailsDTO dto)
        {
            try
            {
                dto.RegisterBy = IsResidentAdmin();
                if (IsResidentAdmin() || IsCSAAdmin())
                {
                    dto.VisitorAccessTypeId = 2;
                }
                var createdVisitor = await _visitorService.SaveVisitorMobileDetailsAsync(dto);
                return CreatedAtAction(nameof(GetVisitorById), new { id = createdVisitor.Id }, createdVisitor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> sendVistorQREmail(QRImageModel dto)
        {
            try
            {
               await _visitorService.SendVisitorQREmail(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommunityAdminVisitors()
        {
            try
            {
                var visitorList = await _visitorService.GetAllCommunityAdminVisitorsAsync();
                return Ok(visitorList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVisitorByVehicleNo(string vehicleNo)
        {
            try
            {
                var visitor = await _visitorService.GetVisitorByVehicleNoAsync(vehicleNo);
                return Ok(visitor);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
