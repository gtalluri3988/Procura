using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Models;
using YourNamespace.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic.Services;
using DB.Entity;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ResidentUploadHistoryController : AuthorizedCSABaseAPIController
    {
        private readonly ICurrentUserService _currentUserService;
        public readonly ICommunityService _communityService;
        public readonly IResidentService _residentService;
        public readonly IResidentUploadHistoryService _residentUploadHistoryService;
        public ResidentUploadHistoryController(
            ICurrentUserService currentUserService,
            ICommunityService communityService,
            IResidentService residentService,
            IUserService userService,
            IResidentUploadHistoryService residentUploadHistoryService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _communityService = communityService;
            _residentService = residentService;
            _residentUploadHistoryService = residentUploadHistoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllResidentsUploadHistory()
        {

            return Ok(await _residentUploadHistoryService.GetAllResidentUploadHistoryAsync());
        }


        [HttpPost]
        public async Task<IActionResult> UploadResidentExcel(IFormFile file, [FromForm] string fileName, [FromForm] string communityId, [FromForm] string jsonData)
        {
            if (file != null)
            {
               
                var parsedJson = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
                try
                {
                    var Msg = await _residentUploadHistoryService.UpdateDataAsync(file, fileName, file.FileName, communityId, parsedJson);
                    return Ok(new { message = "Upload successfully completed" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Upload complete" });
                }
            }
            return BadRequest("No file provided");
        }

        [HttpPost]
        public async Task<IActionResult> ResidentUpload([FromForm] IFormFile file, [FromForm] string fileName,
            [FromForm] string attachment,string communityId, [FromForm] List<Dictionary<string, object>> rows)
        {
            try
            {
               var Msg= await _residentUploadHistoryService.UpdateDataAsync(file,fileName, communityId, attachment,rows);
                return Ok(new { message = "Upload successfully completed" });
            }
            catch (Exception ex) { 
                return BadRequest(new { message = "Upload complete" });
            }
        }
        // [HttpGet]
        //public async Task<IActionResult> GetResidentAccessHistoryByResidentId(int residentId)
        //{
        //    var Residents = await _residentAccessHistoryService.GetResidentsByResidentIdAsync(residentId);
        //    return Ok(await _residentAccessHistoryService.GetResidentsByResidentIdAsync(residentId));
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateResidentAccessHistory(ResidentDTO residentModel)
        //{
        //    Random random = new Random();
        //    var createdResident = await _residentAccessHistoryService.CreateResidentAsync(residentModel);
        //    return CreatedAtAction(nameof(GetResidentAccessHistoryByResidentId), new { id = createdResident.Id }, createdResident);
        //}
        //[HttpPost]
        //public async Task<IActionResult> UpdateResidentAccessHistory(int id, ResidentDTO dto)
        //{
        //    await _residentAccessHistoryService.UpdateResidentAsync(id, dto);
        //    return NoContent();
        //}

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            string drivePath = @"C:\Uploads\ResidentExcel";
            var path = Path.Combine(drivePath, fileName+".xlsx"); // adjust as needed
            if (!System.IO.File.Exists(path))
                return NotFound();
            var bytes = System.IO.File.ReadAllBytes(path);
            var contentType = "application/octet-stream"; // or use MimeMapping
            return File(bytes, contentType, fileName + ".xlsx");
        }
        [HttpGet]
        public IActionResult DownloadTemplate(string fileName)
        {
            string drivePath = @"C:\Uploads\ResidentTemplate";
            var path = Path.Combine(drivePath, "BulkUpload-Template.xlsx"); // adjust as needed
            if (!System.IO.File.Exists(path))
                return NotFound();
            var bytes = System.IO.File.ReadAllBytes(path);
            var contentType = "application/octet-stream"; // or use MimeMapping
            return File(bytes, contentType, "BulkUpload-Template.xlsx");
        }

    }


}
