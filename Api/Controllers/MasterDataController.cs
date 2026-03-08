using Api.Controllers;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Procura.Models;
using YourNamespace.Services;
using static DB.Repositories.MasterDataRepository;

namespace Procura.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MasterDataController : AuthorizedCSABaseAPIController
    {
        private readonly IContentService _contentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IVendorService _vendorService;
        private readonly ISAPServices _sapService;
        private readonly IConfiguration _configuration;
        private readonly IMasterDataService _masterDataService;
        private static readonly HttpClient client = new HttpClient();



        public MasterDataController(IContentService contentService,
            ICurrentUserService currentUserService,
            IUserService userService, IVendorService vendorService, ISAPServices sapServices,
            ILogger<ContentController> logger, IConfiguration configuration, IMasterDataService masterDataService   )
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _contentService = contentService;
            _vendorService = vendorService;
            _sapService = sapServices;
            _configuration = configuration;
            _masterDataService = masterDataService;
        }

        //[AllowAnonymous]
        //[HttpGet("tree/{codeSystemId:int}")]
        //public async Task<IActionResult> GetTree(int codeSystemId)
        //{
        //    var tree = await _masterDataService.GetCodeTreeAsync(codeSystemId);
        //    return Ok(tree);
        //}

        //[AllowAnonymous]
        //[HttpGet("flat/{codeSystemId:int}")]
        //public async Task<IActionResult> GetFlat(int codeSystemId)
        //{
        //    var flat = await _masterDataService.GetCodeHierarchyFlatAsync(codeSystemId);
        //    return Ok(flat);
        //}

        //// Expect a raw JSON array in the request body, e.g. [ { "code":"01", "description":"...", "level":1, "children":[ ... ] } ]
        //[AllowAnonymous]
        //[HttpPost("tree/{codeSystemId:int}")]
        //public async Task<IActionResult> SaveTree(int codeSystemId, [FromBody] List<CodeHierarchyDto> nodes)
        //{
        //    if (nodes == null) return BadRequest("Missing tree payload (expect array of nodes).");
        //    await _masterDataService.SaveCodeTreeAsync(nodes, codeSystemId);
        //    return Ok(new { message = "Tree saved" });
        //}

        //[HttpPost("AddNode/{codeSystemId:int}")]
        //public async Task<IActionResult> AddNode(int codeSystemId, [FromQuery] int? parentId, [FromBody] CodeHierarchyDto node)
        //{
        //    if (node == null) return BadRequest("Missing node payload.");
        //    var created = await _masterDataService.AddNodeAsync(node, codeSystemId, parentId);
        //    return CreatedAtAction(nameof(GetFlat), new { codeSystemId }, created);
        //}

        //[HttpPut("UpdateNode/{id:int}")]
        //public async Task<IActionResult> UpdateNode(int id, [FromBody] CodeHierarchyDto node)
        //{
        //    if (node == null) return BadRequest("Missing node payload.");
        //    await _masterDataService.UpdateNodeAsync(id, node);
        //    return Ok(new { message = "Node updated" });
        //}

        //[HttpDelete("DeleteNode/{id:int}")]
        //public async Task<IActionResult> DeleteNode(int id)
        //{
        //    await _masterDataService.DeleteNodeAsync(id);
        //    return Ok(new { message = "Node deleted" });
        //}

        //[HttpGet("Children/{parentId:int}")]
        //public async Task<IActionResult> GetChildren(int parentId)
        //{
        //    var children = await _masterDataService.GetChildrenAsync(parentId);
        //    return Ok(children);
        //}


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var data = await _masterDataService.GetAllHierarchyAsync();
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveHierarchy(int monthSetting,int YearSetting,[FromBody] List<CategoryDto> categories)
        {
            await _masterDataService.SaveHierarchyAsync(monthSetting, YearSetting, categories);
            return Ok("Saved Successfully");
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCategoryListByMasterCodeId(int codeMasterId)
        {
            var data = await _masterDataService.GetAllHierarchyAsync(codeMasterId);
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveTenderManagement(TenderManagementSaveRequest request)
        {
            await _masterDataService.SaveTenderManagementAsync(request);
            return Ok("Saved Successfully");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTenderManagement()
        {
            var result = await _masterDataService.GetTenderManagementAsync();
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SaveMaterialBudget(MaterialBudgetDto request)
        {
            await _masterDataService.AddMaterilBudgetAsync(request);
            return Ok("Saved Successfully");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpdateMaterialBudget(MaterialBudgetDto request)
        {
            await _masterDataService.UpdateMaterilBudgetAsync(request);
            return Ok("Saved Successfully");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMaterialBudgetList()
        {
           var materialBudget= await _masterDataService.GetAllMaterilBudgetListAsync();
            return Ok(materialBudget);
        }



        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialBudge(int id)
        {
            var materialBudget = await _masterDataService.DeleteMaterilBudgetAsync(id);
            return Ok(materialBudget);
        }

        [AllowAnonymous]
        // POST api/MasterData/UploadMaterialBudgetFiles/123?uploadedBy=5
        [HttpPost]
        public async Task<IActionResult> UploadMaterialBudgetFiles([FromForm] List<IFormFile> files, [FromQuery] int uploadedBy)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

           await _masterDataService.UploadMaterialBudgetFilesAsync(files, uploadedBy);
            return Ok("Files uploaded success");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMaterialBudgetUploads()
        {
            var uploads = await _masterDataService.GetMaterialBudgetUploadsAsync();
            return Ok(uploads);
        }


        [AllowAnonymous]
        // POST api/MasterData/UploadMaterialBudgetFiles/123?uploadedBy=5
        [HttpPost]
        public async Task<IActionResult> SaveorUpdateVendorManagementSetting(VendorManagementSettingDto dto)
        {
           
            await _masterDataService.SaveOrUpdateVendorManagementSettingAsync(dto);
            return Ok("Files uploaded success");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVendorManagementSetting()
        {
            var uploads = await _masterDataService.GetVendorManagementSettingAsync();
            return Ok(uploads);
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCategoryCodeSetting()
        {
            var settings = await _masterDataService.GetCategoryCodeSettingAsync();
            return Ok(new {monthSetting=settings.monthsSetting,yearSetting=settings.yearsetting });
        }
    }
}
