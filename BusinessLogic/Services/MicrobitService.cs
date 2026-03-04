using Azure.Core;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class MicrobitService : IMicrobitService
    {
        private readonly IMicrobitRepository _repository;
        private readonly ILogger<MicrobitService> _logger;

        public MicrobitService(IMicrobitRepository repository, ILogger<MicrobitService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<MaintenanceStatusResponse> GetMaintenanceStatusAsync(MaintenanceStatusRequest request)
        {
            var vehicles = await _repository.GetMaintenanceStatusAsync(request.CommunityId);
            var AllowAccess = await _repository.GetCommunityAllowAccess(request.CommunityId);
            return new MaintenanceStatusResponse
            {
                CommunityId = request.CommunityId,
                OverdueAccess = AllowAccess == "1" ? "Yes" : "No",
                VehicleProfiles = vehicles
            };
        }

        public async Task<VehicleEntryExitResponse> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        {
            var success = await _repository.UpdateVehicleEntryExitAsync(request);
            return new VehicleEntryExitResponse
            {
                CommunityId = request.CommunityId,
                Status = success ? "Successful" : "Failed"
            };
        }



        public async Task<ResidentFaceStatusResponse> GetResidentFaceStatusAsync(ResidentFaceStatusRequest request)
        {
            var profiles = await _repository.GetResidentFaceStatusAsync(request.CommunityId);
            return new ResidentFaceStatusResponse
            {
                CommunityId = request.CommunityId,
                FaceProfiles = profiles
            };
        }

        public async Task<VehicleEntryExitResponse> UpdateResidentFaceEntryExitAsync(ResidentFaceEntryExitRequest request)
        {
            bool success = await _repository.UpdateResidentFaceEntryExitAsync(request);
            return new VehicleEntryExitResponse
            {
                CommunityId = request.CommunityId,
                Status = success ? "Successful" : "Failed"
            };
        }

        // Visitor – QR Code
        public async Task<VisitorQrListResponse> GetVisitorQrListAsync(VisitorQrListRequest request)
        {
            var qrs = await _repository.GetVisitorQrListAsync(request.CommunityId);
            return new VisitorQrListResponse
            {
                CommunityId = request.CommunityId,
                VisitorQrs = qrs
            };
        }

        public async Task<VisitorQrStatusCheckResponse> CheckVisitorQrStatusAsync(VisitorQrStatusCheckRequest request)
        {
            bool active = await _repository.CheckVisitorQrStatusAsync(request.CommunityId, request.QrCodeString);
            return new VisitorQrStatusCheckResponse
            {
                CommunityId = request.CommunityId,
                QrCodeString = request.QrCodeString,
                IsActiveVisitor = active
            };
        }

        public async Task<VehicleEntryExitResponse> UpdateVisitorQrEntryExitAsync(VisitorQrEntryExitRequest request)
        {
            bool success = await _repository.UpdateVisitorQrEntryExitAsync(request);
            return new VehicleEntryExitResponse
            {
                CommunityId = request.CommunityId,
                Status = success ? "Successful" : "Failed"
            };
        }

        public async Task<List<VisitorPlateProfile>> GetVisitorListAsync(string communityId)
        {
            return await _repository.GetVisitorListAsync(communityId);
            
        }

        public async Task<VisitorPlateProfile?> GetRegisteredVisitorAsync(string communityId, string vehiclePlateNo)
        {
            return await _repository.GetRegisteredVisitorAsync(communityId, vehiclePlateNo);
        }
    }
}
