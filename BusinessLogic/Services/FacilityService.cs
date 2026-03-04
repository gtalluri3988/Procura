using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FacilityService : IFacilityService
    {

        private readonly IFacilityRepository _facilityRepository;

        public FacilityService(IFacilityRepository facilityRepository)
        {
            _facilityRepository = facilityRepository;
        }

        public async Task<IEnumerable<FacilityDTO>> GetAllFacilityAsync()
        {
            return await _facilityRepository.GetAllFacilitiesAsync();
        }
        public async Task<IEnumerable<FacilityDTO>> SearchFacilityAsync(int communityId,int facilityId)
        {
            return await _facilityRepository.SearchFacilitiesAsync(communityId, facilityId);
        }
        public async Task<FacilityDTO> GetFacilityByIdAsync(int id)
        {
            return await _facilityRepository.GetAllFacilityByIdAsync(id);
        }

        public async Task<FacilityDTO> GetAllFacilityByAvilableLotQtyIdAsync(int id)
        {
            return await _facilityRepository.GetAllFacilityByAvilableLotQtyIdAsync(id);
        }
        
        public async Task<string> GetLotAvilabilityByMonth(string startMonth, int facilityId)
        {
            return await _facilityRepository.GetLotAvilabilityByMonth(startMonth,facilityId);
        }
        public async Task<FacilityDTO> CreateFacilityAsync(FacilityDTO dto)
        {
            return await _facilityRepository.CreateFacilityAsync(dto);
        }

        public async Task UpdateFacilityAsync(int id, FacilityDTO dto)
        {
            await _facilityRepository.UpdateFacilityAsync(id, dto);
        }

        public async Task DeleteFacilityAsync(int id)
        {
            await _facilityRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByCommunityAsync(int communityId)
        {
            return await _facilityRepository.GetAllFacilitiesByCommunityAsync(communityId);
        }
        
        public async Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByfacilityTypeAsync(int communityId,int facilityTypeId)
        {
            return await _facilityRepository.GetAllFacilitiesByfacilityTypeAsync(communityId, facilityTypeId);
        }
        public async Task<bool> DeleteFacility(int facilityId)
        {
            return await _facilityRepository.DeleteFacility(facilityId);
        }

        public async Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByCommunityAsync(int communityId)
        {
            return await _facilityRepository.GetAllFacilityHistoryByCommunityAsync(communityId);
        }
        public async Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByResidentAsync(int residentId)
        {
            return await _facilityRepository.GetAllFacilityHistoryByResidentAsync(residentId);
        }
        
    }
}
