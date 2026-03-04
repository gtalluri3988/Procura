using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ResidentService : IResidentService
    {
        
        private readonly IResidentRepository _residentRepository;
        
        public ResidentService(IResidentRepository residentRepository)
        {
            _residentRepository = residentRepository;
            
        }
        public async Task<IEnumerable<ResidentDTO>> GetAllResidentsByCommunityAsync(int communityId)
        {
            return await _residentRepository.GetAllResidentsByCommunityIdAsync(communityId);
        }

        public async Task<ResidentDTO> GetResidentsByResidentIdAsync(int residentId)
        {
            return await _residentRepository.GetResidentsByResidentIdAsync(residentId);
        }

        public async Task<ResidentDTO> CreateResidentAsync(ResidentDTO dto)
        {
            return await _residentRepository.SaveResidentAsync(dto);
        }

        public async Task UpdateResidentAsync(int id, ResidentDTO dto)
        {
            await _residentRepository.UpdateResidenAsync(id, dto);
        }
        public async Task<ResidentDTO> GetResidentsByNRICAsync(string nric,int communityId)
        {
            return await _residentRepository.GetResidentsByNRICAsync(nric, communityId);
        }

        public async Task<IEnumerable<ResidentDTO>> GetAllResidentsByCommunityDropdownAsync(int communityId, string Type)
        {
            return await _residentRepository.GetResidentsDropdownsAsync(communityId,Type);
        }

        public async Task<ResidentDTO> GetResidentsNameandContactByAddresses(string roadNo, string blockNo, string level, string houseNo)
        {
            return await _residentRepository.GetResidentsNameandContactByAddresses(roadNo, blockNo, level, houseNo);
        }
        public async Task<IEnumerable<ResidentDTO>> SearchResidentsByCommunityIdAsync(ResidentDTO search)
        {
            return await _residentRepository.SearchResidentsByCommunityIdAsync(search);
        }

        public async Task<IEnumerable<string>> GetResidentHierarchyAsync(int communityId, string roadNo, string blockNo, string level, string targetField)
        {
            return await _residentRepository.GetResidentHierarchyAsync(communityId, roadNo, blockNo, level, targetField);
        }

        
        public async Task UpdateResidentSelfieAsync(int id, string? photo)
        {
            await _residentRepository.UpdateResidentVehicleSelfieAsync(id, photo);
        }
        public async Task<string> GetVehicleSelfieByIdAsync(int vehicleId)
        {
            return await _residentRepository.GetVehicleSelfieByIdAsync(vehicleId);
        }

        public async Task UpdateResidentProfileAsync(int id, ResidentDTO dto)
        {
            await _residentRepository.UpdateResidentProfileAsync(id, dto);
        }

        public async Task UpdateResidentProfileAddressAsync(int id, ResidentDTO dto)
        {
            await _residentRepository.UpdateResidentProfileAddressAsync(id, dto);
        }

        public async Task<bool> DeleteResident(int residentId)
        {
           return await _residentRepository.DeleteResident(residentId);
        }

        public async Task<bool> DeleteVehicle(int vehicleId)
        {
            return await _residentRepository.DeleteVehicle(vehicleId);
        }

        public async Task<bool> SaveResidentVehicleAsync(VehicleModelDTO vehicleDetails)
        {
            return await _residentRepository.SaveResidentVehicleAsync(vehicleDetails);
        }
        public async Task UpdateResidenByAdminAsync(int residentId, ResidentDTO resident)
        {
             await _residentRepository.UpdateResidenByAdminAsync(residentId,resident);
        }
    }
}
