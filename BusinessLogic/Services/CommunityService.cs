using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using DB.EFModel;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommunityRepository _communityRepository;
        public CommunityService(IUserRepository userRepository,ICommunityRepository communityRepository)
        {
            _userRepository = userRepository;
            _communityRepository = communityRepository;
        }
       

        public async Task<List<CommunityObject>> GetCommunityList()
        {
            var CommunityList =await _communityRepository.GetCommunityListAsync();
            return CommunityList?.Select(item => new CommunityObject
            {
                Id = item?.Id ?? 0,  // Default to 0 if null
                CommunityId = item?.CommunityId ?? string.Empty,  // Default to empty string
                CommunityName = item?.CommunityName ?? "Unknown",
                City = item?.CityName ?? "N/A",
                State = item?.StateId ?? 0,
                Address = item?.Address ?? "No Address",
                NoOfResidentParkingLot = item?.NoOfParkingLot ?? 0,
                NoOfUnits = item?.NoOfUnits ?? 0,
                PICEmail = item?.PICEmail ?? string.Empty,
                PICPhone = item?.PICMobile ?? string.Empty,
                PICName = item?.PICName ?? "No Contact"
            }).ToList() ?? new List<CommunityObject>(); // Return empty list if `CommunityList` is null

        }

        public async Task<List<CommunityTypeDto>> GetCommunityTypeList()
        {
            var CommunityList = await _communityRepository.GetCommunityTypeAsync();
            return CommunityList?.Select(item => new CommunityTypeDto
            {
                Id = item?.Id ?? 0,  // Default to 0 if null
                Name = item?.Name ?? string.Empty,  
                
            }).ToList() ?? new List<CommunityTypeDto>(); 

        }

        public async Task<IEnumerable<CommunityDTO>> GetAllCommunitiesAsync()
        {
            return await _communityRepository.GetAllAsync();
        }

        public async Task<CommunityDTO> GetCommunityByIdAsync(int id)
        {
            return await _communityRepository.GetCommunityByIdAsync(id);
        }

        public async Task<CommunityDTO> CreateCommunityAsync(CommunityDTO dto)
        {
            return await _communityRepository.SaveCommunityAsync(dto);
        }

        public async Task<string> GetNextNumberAsync()
        {
            return await _communityRepository.IncrementAndGetNextNumberAsync();
            
        }

        public async Task UpdateCommunityAsync(int id, CommunityDTO dto)
        {
            await _communityRepository.UpdateCommunityAsync(id, dto);
        }

        public async Task DeleteCommunityAsync(int id)
        {
            await _communityRepository.DeleteCommunity(id);
        }

        public async Task<IEnumerable<CommunityDTO>> GetAllCommunitiesWithStatesAsync()
        {
            return await _communityRepository.GetAllWithStatesAsync();
        }

        public async Task<IEnumerable<CommunityResidentCountDto>> GetCommunitiesWithResidentCountAsync()
        {
            return await _communityRepository.GetAllCommunityWithResidentListAsync();
        }
        public async Task<IEnumerable<DropDownDTO>> GetCityByStateAsync(int stateId)
        {
            return await _communityRepository.GetCityByStateAsync(stateId);
        }

    }
}
