using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityDTO>> GetAllFacilityAsync();
        Task<FacilityDTO> GetFacilityByIdAsync(int id);
        Task<FacilityDTO> CreateFacilityAsync(FacilityDTO dto);
        Task UpdateFacilityAsync(int id, FacilityDTO dto);
        Task DeleteFacilityAsync(int id);
        Task<IEnumerable<FacilityDTO>> SearchFacilityAsync(int communityId, int facilityId);

        Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByCommunityAsync(int communityId);

        Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByfacilityTypeAsync(int communityId, int facilityTypeId);
        Task<FacilityDTO> GetAllFacilityByAvilableLotQtyIdAsync(int id);
        Task<string> GetLotAvilabilityByMonth(string startMonth, int facilityId);
        Task<bool> DeleteFacility(int facilityId);
        Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByCommunityAsync(int communityId);
        Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByResidentAsync(int residentId);
    }
}
