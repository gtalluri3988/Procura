using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<FacilityDTO>> GetAllAsync();
        Task<FacilityDTO> GetByIdAsync(int id);
        Task<FacilityDTO> AddAsync(FacilityDTO dto);
        Task UpdateAsync(int id, FacilityDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<FacilityDTO>> GetAllFacilitiesAsync();
        Task<FacilityDTO> CreateFacilityAsync(FacilityDTO dto);
        Task<FacilityDTO> GetAllFacilityByIdAsync(int id);
        Task UpdateFacilityAsync(int facilityId, FacilityDTO facility);
        //Task UpdateFacilityAsync(int facilityId, FacilityDTO facility);
        Task<IEnumerable<FacilityDTO>> SearchFacilitiesAsync(int communityId, int facilityTypeId);
        Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByCommunityAsync(int communityId);

        Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByfacilityTypeAsync(int communityId, int facilityTypeId);
        Task<FacilityDTO> GetAllFacilityByAvilableLotQtyIdAsync(int id);
        Task<string> GetLotAvilabilityByMonth(string startMonth, int facilityId);
        Task<bool> DeleteFacility(int facilityId);
        Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByCommunityAsync(int communityId);
        Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByResidentAsync(int residentId);
    }
}
