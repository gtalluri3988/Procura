using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IMicrobitRepository
    {
        Task<List<VehicleProfile>> GetMaintenanceStatusAsync(string communityId);
        Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request);


        // Resident - Face
        Task<List<ResidentFaceProfile>> GetResidentFaceStatusAsync(string communityId);
        Task<bool> UpdateResidentFaceEntryExitAsync(ResidentFaceEntryExitRequest request);

        // Visitor - QR
        Task<List<VisitorQrProfile>> GetVisitorQrListAsync(string communityId);
        Task<bool> CheckVisitorQrStatusAsync(string communityId, string qrCodeString);
        Task<bool> UpdateVisitorQrEntryExitAsync(VisitorQrEntryExitRequest request);

        Task<List<VisitorPlateProfile>> GetVisitorListAsync(string communityId);

        Task<VisitorPlateProfile?> GetRegisteredVisitorAsync(string communityId, string vehiclePlateNo);

        Task<string> GetCommunityAllowAccess(string communityId);
    }
}
