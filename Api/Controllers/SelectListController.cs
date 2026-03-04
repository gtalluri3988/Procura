using BusinessLogic.Interfaces;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SelectListController : ControllerBase
    {
        private readonly IDropDownService _dropDownService;
        public SelectListController(IDropDownService dropdownService)
        {
            _dropDownService = dropdownService;
        }
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetSelectListAsync(string inputTypes)
        //{
        //    return Ok(await _dropDownService.GetSelectList(inputTypes));
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSelectListAsync(string inputTypes)
        {
            if (string.IsNullOrWhiteSpace(inputTypes))
                return BadRequest("inputTypes is required.");

            // Allow comma-separated input like "VendorType,State,RegistrationStatus"
            var types = inputTypes
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();

            if (types.Length == 0)
                return Ok(new Dictionary<string, List<DropdownItem>>());

            // Backwards-compatible: single type returns the list directly
            if (types.Length == 1)
            {
                var list = await _dropDownService.GetSelectList(types[0]);
                return Ok(list ?? new List<DropdownItem>());
            }

            // Multiple types: fetch concurrently and return a map { type -> items }
            var tasks = types.Select(t => _dropDownService.GetSelectList(t)).ToArray();
            var results = await Task.WhenAll(tasks);

            var map = new Dictionary<string, List<DropdownItem>>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < types.Length; i++)
            {
                map[types[i]] = results[i] ?? new List<DropdownItem>();
            }

            return Ok(map);
        }


    }
}
