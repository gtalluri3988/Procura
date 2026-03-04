using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IResidentAccessHistoryService
    {
        Task UpdateResidentAccessHistoryAsync(int id, ResidentAccessHistoryDTO dto);

        Task<ResidentAccessHistoryDTO> SaveResidentAccessHistoryAsync(ResidentAccessHistoryDTO resident);

        Task<ResidentAccessHistoryDTO> GetResidentAccessHistoryByIdAsync(int id);

        Task<IEnumerable<ResidentAccessHistoryDTO>> GetAllResidentAccessHistoryAsync(int? communityId, bool isCSAAdmin);

        Task<ResidentAccessHistoryDTO> GetResidentAccessHistoryByIdAsync(int? AccessId);
        Task<IEnumerable<ResidentAccessHistoryDTO>> SearchResidentAccessHistoryBySearchParamsAsync(ResidentAccessHistoryDTO searchModel);

    }
}
