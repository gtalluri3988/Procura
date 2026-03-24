using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ITenderService
    {
        Task<int> AddTenderApplicationAsync(TenderApplicationDto dto);
        Task<TenderApplicationDto> UpdateTenderApplicationAsync(TenderApplicationDto dto);
        Task DeleteTenderApplicationAsync(int tenderApplicationId);

        Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync();

        Task<TenderApplicationDto?> GetTenderApplicationByIdAsync(int id);

        Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync(
          int? applicationLevelId,
          int? tenderCategoryId,
          int? jobCategoryId,
          int? statusId);
        Task<IEnumerable<UserDTO>> GetTendorReviewers(int ApplicationLevelId, int DesignationId, int StateId);
    }
}
