using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class TenderService : ITenderService
    {
        private readonly ITenderRepository _tenderRepository;

        public TenderService(ITenderRepository tenderRepository)
        {
            _tenderRepository = tenderRepository;
        }
        public async Task<int> AddTenderApplicationAsync(TenderApplicationDto dto)
        {
            return await _tenderRepository.AddTenderApplicationAsync(dto);
        }

        public async Task DeleteTenderApplicationAsync(int tenderApplicationId)
        {
            await _tenderRepository.DeleteTenderApplicationAsync(tenderApplicationId);
        }

        public async Task<TenderApplicationDto> UpdateTenderApplicationAsync(TenderApplicationDto dto)
        {
            return await _tenderRepository.UpdateTenderApplicationAsync(dto);
        }

        public async Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync()
        {
            return await _tenderRepository.GetAllTenderApplicationsAsync();
        }

        public async Task<TenderApplicationDto?> GetTenderApplicationByIdAsync(int id)
        {
            return await _tenderRepository.GetTenderApplicationByIdAsync(id);
        }

        public async Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync(int? applicationLevelId, int? tenderCategoryId, int? jobCategoryId, int? statusId)
        {
            return await _tenderRepository.GetAllTenderApplicationsAsync(applicationLevelId, tenderCategoryId, jobCategoryId, statusId);
        }

        public async Task<IEnumerable<UserDTO>> GetTendorReviewers(int ApplicationLevelId, int DesignationId, int StateId)
        {
            return await _tenderRepository.GetTendorReviewers(ApplicationLevelId, DesignationId, StateId);
        }

        public async Task SaveTenderAdvertisementPageAsync(TenderAdvertisementPageDto dto)
        {
            await _tenderRepository.SaveTenderAdvertisementPageAsync(dto);
        }

        public async Task<TenderAdvertisementPageDto?> GetTenderAdvertisementPageByTenderApplicationIdAsync(int tenderApplicationId)
        {
            return await _tenderRepository.GetTenderAdvertisementPageByTenderApplicationIdAsync(tenderApplicationId);
        }

        public async Task<IEnumerable<UserDTO>> SearchCommitteeUsersAsync(string name, string committeeType)
        {
            return await _tenderRepository.SearchCommitteeUsersAsync(name, committeeType);
        }

        public async Task<List<TenderOpeningListDto>> GetTenderOpeningListAsync(string? referenceId, string? projectName)
        {
            return await _tenderRepository.GetTenderOpeningListAsync(referenceId, projectName);
        }

        public async Task<TenderOpeningDetailDto?> GetTenderOpeningDetailAsync(int tenderId)
        {
            return await _tenderRepository.GetTenderOpeningDetailAsync(tenderId);
        }

        public async Task<TenderOpeningPageDto?> GetTenderOpeningPageAsync(int tenderId)
        {
            return await _tenderRepository.GetTenderOpeningPageAsync(tenderId);
        }

        public async Task<TenderEvaluationPageDto?> GetTenderEvaluationPageAsync(int tenderId)
        {
            return await _tenderRepository.GetTenderEvaluationPageAsync(tenderId);
        }

        public async Task<TenderTechnicalEvalPopupDto?> GetTechnicalEvaluationPopupAsync(int tenderId, int vendorId)
        {
            return await _tenderRepository.GetTechnicalEvaluationPopupAsync(tenderId, vendorId);
        }

        public async Task SaveTechnicalScoreAsync(SaveTechnicalScoreDto dto)
        {
            await _tenderRepository.SaveTechnicalScoreAsync(dto);
        }

        public async Task SaveTenderRecommendationAsync(TenderRecommendationPageDto dto)
        {
            await _tenderRepository.SaveTenderRecommendationAsync(dto);
        }

        public async Task<List<TenderAwardListDto>> GetTenderAwardListAsync(string? referenceId, string? projectName)
        {
            return await _tenderRepository.GetTenderAwardListAsync(referenceId, projectName);
        }

        public async Task<TenderAwardPageDto?> GetTenderAwardPageAsync(int tenderId)
        {
            return await _tenderRepository.GetTenderAwardPageAsync(tenderId);
        }

        public async Task SaveTenderAwardAsync(SaveTenderAwardDto dto)
        {
            await _tenderRepository.SaveTenderAwardAsync(dto);
        }

        public async Task<TenderAwardMinutesDto> SaveTenderAwardMinutesAsync(SaveTenderAwardMinutesDto dto)
        {
            return await _tenderRepository.SaveTenderAwardMinutesAsync(dto);
        }

        public async Task DeleteTenderAwardMinutesAsync(int minutesId)
        {
            await _tenderRepository.DeleteTenderAwardMinutesAsync(minutesId);
        }

        public async Task<VendorPerformancePageDto?> GetVendorPerformancePageAsync(int tenderId)
        {
            return await _tenderRepository.GetVendorPerformancePageAsync(tenderId);
        }

        public async Task SaveVendorPerformanceAsync(SaveVendorPerformanceDto dto, int userId)
        {
            await _tenderRepository.SaveVendorPerformanceAsync(dto, userId);
        }

        // Approval Workflow
        public async Task<List<TenderApprovalWorkflowDto>> GetApprovalWorkflowAsync(int tenderId)
        {
            return await _tenderRepository.GetApprovalWorkflowAsync(tenderId);
        }

        public async Task InitializeWorkflowAsync(int tenderId, decimal? estimatedPrices, int siteLevelId, int siteOfficeId)
        {
            await _tenderRepository.InitializeWorkflowAsync(tenderId, estimatedPrices, siteLevelId, siteOfficeId);
        }

        public async Task ApproveRejectWorkflowAsync(int tenderId, string stage, int level, string status, string? remarks, int userId)
        {
            await _tenderRepository.ApproveRejectWorkflowAsync(tenderId, stage, level, status, remarks, userId);
        }

        public async Task ChangeWorkflowApproverAsync(int tenderId, string stage, int level, int newUserId, string? changeRemarks)
        {
            await _tenderRepository.ChangeWorkflowApproverAsync(tenderId, stage, level, newUserId, changeRemarks);
        }
    }
}
