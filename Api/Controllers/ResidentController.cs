using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using BusinessLogic.Models.Users;
using BusinessLogic.Services;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using YourNamespace.Services;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ResidentController : AuthorizedCSABaseAPIController
    {
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public readonly IResidentService _residentService;
        public ResidentController(
            ICurrentUserService currentUserService, 
            ICommunityService communityService,
            IResidentService residentService,
            IUserService userService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService = communityService;
            _residentService = residentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllResidentsByCommunity(int communityId)
        {
            try
            {
                return Ok(await _residentService.GetAllResidentsByCommunityAsync(communityId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetResidentsByResidentId(int residentId)
        {
            var Residents = await _residentService.GetResidentsByResidentIdAsync(residentId);
            return Ok(await _residentService.GetResidentsByResidentIdAsync(residentId));
        }
      
        [HttpPost]
        public async Task<IActionResult> CreateResident(ResidentDTO residentModel)
        {
            try
            {
                Random random = new Random();
                var createdResident = await _residentService.CreateResidentAsync(residentModel);
                return CreatedAtAction(nameof(GetResidentsByResidentId), new { id = createdResident.Id }, createdResident);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateResident(int id, ResidentDTO dto)
        {
            try
            {
                await _residentService.UpdateResidentAsync(id, dto);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateResidentByAdmin(int id, ResidentDTO dto)
        {
            try
            {
                await _residentService.UpdateResidenByAdminAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UpdateResident(int id, [FromForm] ResidentDTO model)
        //{
        //    if (model.File != null)
        //    {
        //        var uploadsPath = Path.Combine("C:\\Uploads\\ResidentVehicles");
        //        Directory.CreateDirectory(uploadsPath);

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
        //        var filePath = Path.Combine(uploadsPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.File.CopyToAsync(stream);
        //        }

        //        model.FileName = fileName; // save new file name in DB if needed

        //    }
        //    await _residentService.UpdateResidentAsync(id, model);

        //    // Update DB logic here...
        //    return Ok(new { message = "Profile updated successfully", model.FileName });
        //}

        [HttpGet]
        public async Task<IActionResult> GetResidentByNric(string nric,int communityId)
        {
            return Ok(await _residentService.GetResidentsByNRICAsync(nric, communityId));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllResidentsByCommunityDropdown(int communityId)
        {

            return Ok(await _residentService.GetAllResidentsByCommunityDropdownAsync(communityId, ""));
        }


        [HttpGet]
        public async Task<IActionResult> GetResidentsNameandContactByAddresses(string roadNo, string? blockNo, string? level, string houseNo)
        {
            return Ok(await _residentService.GetResidentsNameandContactByAddresses(roadNo, blockNo, level, houseNo));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllResidentsBysearchParams(ResidentDTO Params)
        {

            return Ok(await _residentService.SearchResidentsByCommunityIdAsync(Params));
        }

        //[HttpPost]
        //public async Task<IActionResult> GetResidentHierarchy(int communityId, string roadNo = null, string blockNo = null, string level = null, string targetField = "RoadNo")
        //{
        //    var result = await _residentService.GetResidentHierarchyAsync(communityId, roadNo, blockNo, level, targetField);
        //    return Ok(result);
        //}

        [HttpPost]
        public async Task<IActionResult> GetResidentHierarchy([FromBody] ResidencyHierarchyModel request)
        {
            var result = await _residentService.GetResidentHierarchyAsync(
                request.CommunityId,
                request.RoadNo,
                request.BlockNo,
                request.Level,
                request.TargetField
            );
            return Ok(result);
        }

        
        [HttpPost]
        public async Task<IActionResult> UploadSelfie(Api.Models.SelfieUploadModel model)
        {
            try
            {
                await _residentService.UpdateResidentSelfieAsync(model.ResidentId,model.ImageBase64);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Image captured failed");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleImage(int residentId)
        {
            Api.Models.SelfieUploadModel model = new Models.SelfieUploadModel();
            model.ResidentId= residentId;
            model.ImageBase64 = await _residentService.GetVehicleSelfieByIdAsync(residentId);

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResidentProfile(int id, ResidentDTO dto)
        {
            await _residentService.UpdateResidentProfileAsync(id, dto);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResidentProfileAddress(int id, ResidentDTO dto)
        {
            await _residentService.UpdateResidentProfileAddressAsync(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteResident(int residentId)
        {
            try
            {
                await _residentService.DeleteResident(residentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveResidentVehicleDetails(VehicleModelDTO dto)
        {
            await _residentService.SaveResidentVehicleAsync(dto);
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            try
            {
                await _residentService.DeleteVehicle(vehicleId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetBase64Image(string filename)
        {
            string file = Path.GetFileName(filename);
            var filePath = Path.Combine(@"C:\Uploads\ResidentVehicles", file);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            var base64 = Convert.ToBase64String(bytes);
            return Ok(base64);
        }


    }
}
