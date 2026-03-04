using BusinessLogic.Interfaces.Entities;
using BusinessLogic.Models;
using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICommunityService
    {
        Task<List<CommunityObject>> GetCommunityList();
        Task<List<CommunityTypeDto>> GetCommunityTypeList();

        Task<IEnumerable<CommunityDTO>> GetAllCommunitiesAsync();
        Task<CommunityDTO> GetCommunityByIdAsync(int id);
        Task<CommunityDTO> CreateCommunityAsync(CommunityDTO dto);
        Task UpdateCommunityAsync(int id, CommunityDTO dto);
        Task DeleteCommunityAsync(int id);
        Task<IEnumerable<CommunityDTO>> GetAllCommunitiesWithStatesAsync();

        Task<IEnumerable<CommunityResidentCountDto>> GetCommunitiesWithResidentCountAsync();
        Task<string> GetNextNumberAsync();
        Task<IEnumerable<DropDownDTO>> GetCityByStateAsync(int stateId);
    }
}
