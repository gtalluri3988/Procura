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
    public class MicrobitApiController : AuthorizedCSABaseAPIController
    {
        private readonly IVisitorService _visitorService;
        private readonly IMicrobitService _microbitService;
        public MicrobitApiController(IVisitorService visitorService, IMicrobitService microbitService,
            IUserService userService,
            ILogger<ResidentController> logger
            ) : base(userService, logger)
        {
            _visitorService = visitorService;
            _microbitService = microbitService;

        }

        [HttpPost]
        public async Task<IActionResult> GetMaintenanceBillStatus([FromBody] MaintenanceStatusRequest request)
        {
            var result = await _microbitService.GetMaintenanceStatusAsync(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVehicleEntryExit([FromBody] VehicleEntryExitRequest request)
        {
            var result = await _microbitService.UpdateVehicleEntryExitAsync(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetResidentImage([FromBody] ResidentFaceStatusRequest request)
    => Ok(await _microbitService.GetResidentFaceStatusAsync(request));

        //[HttpPost]
        //public async Task<IActionResult> UpdateResidentFaceEntryExit([FromBody] ResidentFaceEntryExitRequest request)
        //    => Ok(await _microbitService.UpdateResidentFaceEntryExitAsync(request));

        [HttpPost]
        public async Task<IActionResult> GetVisitorQrList([FromBody] VisitorQrListRequest request)
            => Ok(await _microbitService.GetVisitorQrListAsync(request));

        //[HttpPost]
        //public async Task<IActionResult> CheckVisitorQrStatus([FromBody] VisitorQrStatusCheckRequest request)
        //    => Ok(await _microbitService.CheckVisitorQrStatusAsync(request));
        //[HttpPost]
        //public async Task<IActionResult> UpdateVisitorQrEntryExit([FromBody] VisitorQrEntryExitRequest request)
        //    => Ok(await _microbitService.UpdateVisitorQrEntryExitAsync(request));

        [HttpPost]
        public async Task<IActionResult> GetVisitorList([FromBody] VisitorQrListRequest request)
           => Ok(await _microbitService.GetVisitorListAsync(request.CommunityId));

        [HttpPost]
        public async Task<IActionResult> GetRegisteredVisitor([FromBody] VisitorQrListRequest request)
           => Ok(await _microbitService.GetRegisteredVisitorAsync(request.CommunityId,request.VehiclePlateNo));

    }
}
