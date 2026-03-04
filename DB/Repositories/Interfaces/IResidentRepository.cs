using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IResidentRepository
    {

        Task<IEnumerable<ResidentDTO>> GetAllAsync();
        Task<ResidentDTO> GetByIdAsync(int id);
        Task<ResidentDTO> AddAsync(ResidentDTO dto);
        Task UpdateAsync(int id, ResidentDTO dto);
        Task DeleteAsync(int id);

        Task<IEnumerable<ResidentDTO>> GetAllResidentsByCommunityIdAsync(int communityId);
        Task<ResidentDTO> GetResidentsByResidentIdAsync(int residentId);

        Task<ResidentDTO> SaveResidentAsync(ResidentDTO resident);
        Task UpdateResidenAsync(int residentId, ResidentDTO resident);
        Task<ResidentDTO> GetResidentsByNRICAsync(string nric, int communityId);
        ResidentDTO GetResidentsByEmailPasswordAsync(string Email, string Password);
        Task<List<ResidentDTO>> GetResidentsDropdownsAsync(int communityId, string Type);
        Task<ResidentDTO> GetResidentsNameandContactByAddresses(string roadNo, string blockNo, string level, string houseNo);
        Task<IEnumerable<ResidentDTO>> SearchResidentsByCommunityIdAsync(ResidentDTO searchModel);
        Task<List<string>> GetResidentHierarchyAsync(int communityId = 0,string roadNo = null, string blockNo = null, string level = null, string targetField = "RoadNo");
        Task UpdateResidentVehicleSelfieAsync(int vehicleId, string image);
        Task<string> GetVehicleSelfieByIdAsync(int id);
        Task UpdateResidentProfileAsync(int residentId, ResidentDTO resident);
        Task UpdateResidentProfileAddressAsync(int residentId, ResidentDTO resident);
        Task<bool> DeleteResident(int residentId);
        Task<bool> SaveResidentVehicleAsync(VehicleModelDTO vehicleDetails);
        Task<bool> DeleteVehicle(int vehicleId);
        Task UpdateResidenByAdminAsync(int residentId, ResidentDTO resident);


    }
}
