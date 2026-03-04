using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IMicrobitService
    {
        Task<MaintenanceStatusResponse> GetMaintenanceStatusAsync(MaintenanceStatusRequest request);
        Task<VehicleEntryExitResponse> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request);

        Task<ResidentFaceStatusResponse> GetResidentFaceStatusAsync(ResidentFaceStatusRequest request);
        Task<VehicleEntryExitResponse> UpdateResidentFaceEntryExitAsync(ResidentFaceEntryExitRequest request);

        // Visitor - QR
        Task<VisitorQrListResponse> GetVisitorQrListAsync(VisitorQrListRequest request);
        Task<VisitorQrStatusCheckResponse> CheckVisitorQrStatusAsync(VisitorQrStatusCheckRequest request);
        Task<VehicleEntryExitResponse> UpdateVisitorQrEntryExitAsync(VisitorQrEntryExitRequest request);
        Task<List<VisitorPlateProfile>> GetVisitorListAsync(string communityId);
        Task<VisitorPlateProfile?> GetRegisteredVisitorAsync(string communityId, string vehiclePlateNo);


    }
}
