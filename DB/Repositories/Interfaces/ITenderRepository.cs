using DB.EFModel;
using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface ITenderRepository
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






        Task<TenderAdvertisementSetting> GetByIdAsync(int id);
        Task<List<TenderAdvertisementSetting>> GetAllAsync();

        Task<int> CreateAsync(TenderAdvertisementSetting tender);
        Task UpdateAsync(TenderAdvertisementSetting tender);
        Task DeleteAsync(int id);

        // Committees
        Task AddOpeningCommitteeAsync(TenderOpeningCommittee entity);
        Task AddEvaluationCommitteeAsync(TenderEvaluationCommittee entity);

        // Criteria
        Task AddCriteriaAsync(TenderEvaluationCriteria criteria);
        Task AddSpecificationAsync(TenderEvaluationSpecification spec);

        // Approval
        Task AddApprovalAsync(TenderApproval approval);





    }
}
