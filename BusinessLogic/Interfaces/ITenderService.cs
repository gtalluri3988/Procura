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

        // Advertisement Page
        Task SaveTenderAdvertisementPageAsync(TenderAdvertisementPageDto dto);
        Task<TenderAdvertisementPageDto?> GetTenderAdvertisementPageByTenderApplicationIdAsync(int tenderApplicationId);

        // Search users by name for committee assignment
        Task<IEnumerable<UserDTO>> SearchCommitteeUsersAsync(string name, string committeeType);

        // Tender Opening flow
        Task<List<TenderOpeningListDto>> GetTenderOpeningListAsync(string? referenceId, string? projectName);
        Task<TenderOpeningDetailDto?> GetTenderOpeningDetailAsync(int tenderId);
        Task<TenderOpeningPageDto?> GetTenderOpeningPageAsync(int tenderId);

        // Tender Evaluation flow
        Task<TenderEvaluationPageDto?> GetTenderEvaluationPageAsync(int tenderId);
        Task<TenderTechnicalEvalPopupDto?> GetTechnicalEvaluationPopupAsync(int tenderId, int vendorId);
        Task SaveTechnicalScoreAsync(SaveTechnicalScoreDto dto);
        Task SaveTenderRecommendationAsync(TenderRecommendationPageDto dto);

        // Tender Award flow
        Task<List<TenderAwardListDto>> GetTenderAwardListAsync(string? referenceId, string? projectName);
        Task<TenderAwardPageDto?> GetTenderAwardPageAsync(int tenderId);
        Task SaveTenderAwardAsync(SaveTenderAwardDto dto);
        Task<TenderAwardMinutesDto> SaveTenderAwardMinutesAsync(SaveTenderAwardMinutesDto dto);
        Task DeleteTenderAwardMinutesAsync(int minutesId);

        // Vendor Performance flow
        Task<VendorPerformancePageDto?> GetVendorPerformancePageAsync(int tenderId);
        Task SaveVendorPerformanceAsync(SaveVendorPerformanceDto dto, int userId);

        // Approval Workflow
        Task<List<TenderApprovalWorkflowDto>> GetApprovalWorkflowAsync(int tenderId);
        Task InitializeWorkflowAsync(int tenderId, decimal? estimatedPrices, int siteLevelId, int siteOfficeId);
        Task ApproveRejectWorkflowAsync(int tenderId, string stage, int level, string status, string? remarks, int userId);
        Task ChangeWorkflowApproverAsync(int tenderId, string stage, int level, int newUserId, string? changeRemarks);
        Task InitializeIssuanceWorkflowAsync(int tenderId, int siteLevelId, int siteOfficeId);
        Task<List<TenderEvaluationSpecificationDto>> GetMasterEvaluationCriteriaAsync(int jobCategoryId);
        Task<List<AdvertisedTenderDto>> GetAdvertisedTendersAsync();
        Task AdvertiseTenderAsync(int tenderId);
    }
}
