using BusinessLogic.Models;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using Procura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IVendorService
    {
        Task SaveStepAsync(int vendorId, VendorRegistrationStep step, StepSaveRequest data);
        Task SaveSSMResponse(string input, string response);

        Task<VendorProfileDto> RegisterVendor(CreateVendorRequest request);

        Task SendRegistrationConfirmationAsync(int vendorId);

        Task SendApprovalConfirmationAsync(int vendorId);

        Task SendCategoryCodeApprovalEmailAsync(int vendorId, int approvalRequestId);

        Task SendRenewalReminderAsync(int vendorId, int daysRemaining);

        Task RenewRegistrationAsync(int vendorId);
        Task SendRenewalSuccessAsync(int vendorId);

        Task SaveStepRawAsync(int vendorId, VendorRegistrationStep step, JsonElement data);



        Task<VendorRegistrationStep?> SaveProfileAsync(int vendorId, VendorProfileDto request);
        Task<VendorRegistrationStep?> SaveMembersAsync(int vendorId, VendorMemberDto request);
        Task<VendorRegistrationStep?> SaveFinancialAsync(int vendorId, VendorFinancialDto request);
        Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, VendorCategoryRequest request);
        Task<VendorRegistrationStep?> SaveExperiencesAsync(int vendorId, List<VendorExperienceDto> request);
        Task<VendorRegistrationStep?> SaveDeclarationAsync(int vendorId, VendorDeclarationDto request);
        Task<PaymentDetailsDTO> GetPaymentDetailsAsync(int vendorId);

        Task<IEnumerable<CompanyCategoryDto>> GetCompanyTypes();
        Task<IEnumerable<CompanyCategoryDto>> GetCompanyEntitiesByTypeIdAsync(int TypeId);

        Task<IEnumerable<VendorProfileDto>> GetVendorListAsync();

        Task<VendorDashboardDto> GetVendorDashboardAsync();

        Task<IEnumerable<VendorProfileDto>> GetVendorListAsync(VendorSearchRequest? request);

        Task<IEnumerable<IndustryTypeDto>> BindIndustryTypeListAsync();

        Task<Vendor?> GetVendorFullDetailsAsync(int vendorId);

           Task SaveQuestionAnswers(int vendorId, List<QuestionAnswerDto> answers);
        Task<List<QuestionAnswerDto>> GetQuestionAnswersByQuestionnaireId(int questionnaireId, int vendorId);

        Task<CategoryChangeValidationResult> ValidateCategoryChangeAsync(int vendorId);

        Task<bool> SoftDeleteVendorAsync(int vendorId);

        Task<DateTime?> UpdateLastLoginAsync(int vendorId);
    }
}
