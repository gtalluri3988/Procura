using DB.EFModel;
using DB.Entity;

namespace DB.Repositories.Interfaces
{
    public interface ICommunityRepository
    {
        Task<List<Community>?> GetCommunityListAsync();
        Task<List<CommunityType>> GetCommunityTypeAsync();

        Task<IEnumerable<CommunityDTO>> GetAllAsync();
        Task<CommunityDTO> GetByIdAsync(int id);
        Task<CommunityDTO> AddAsync(CommunityDTO dto);
        Task UpdateAsync(int id, CommunityDTO dto);
        Task DeleteAsync(int id);

        Task<IEnumerable<CommunityDTO>> GetAllWithStatesAsync();
        Task<CommunityDTO> SaveCommunityAsync(CommunityDTO community);
        Task<CommunityDTO> GetCommunityByIdAsync(int id);

        Task UpdateCommunityAsync(int communityId, CommunityDTO community);
        Task<List<CommunityResidentCountDto>> GetAllCommunityWithResidentListAsync();
        Task<string> IncrementAndGetNextNumberAsync();
        Task<IEnumerable<DropDownDTO>> GetCityByStateAsync(int stateId);
        Task<bool> DeleteCommunity(int communityId);
        string GetCommunityNameByIdAsync(int communityId);
    }
}
