using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IResidentAccessHistoryRepository
    {
        Task<IEnumerable<ResidentAccessHistoryDTO>> GetAllAsync();
        Task<ResidentAccessHistoryDTO> GetByIdAsync(int id);
        Task<ResidentAccessHistoryDTO> AddAsync(ResidentAccessHistoryDTO dto);
        Task UpdateAsync(int id, ResidentAccessHistoryDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ResidentAccessHistoryDTO>> GetAllResidentAccessHistoryAsync(int? communityId, bool isCSAAdmin);
        Task<ResidentAccessHistoryDTO> SaveResidentAccessHistoryAsync(ResidentAccessHistoryDTO resident);
        Task<ResidentAccessHistoryDTO> GetResidentAccessHistoryByIdAsync(int? AccessId);
        Task<IEnumerable<ResidentAccessHistoryDTO>> SearchResidentAccessHistoryBySearchParamsAsync(ResidentAccessHistoryDTO searchModel);


    }
}
