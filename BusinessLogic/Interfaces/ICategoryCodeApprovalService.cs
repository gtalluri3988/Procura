using BusinessLogic.Models;
using DB.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICategoryCodeApprovalService
    {
        Task<int> SubmitCategoryChangeRequestAsync(int vendorId, VendorCategoryRequest request);
        Task<List<CategoryCodeApprovalListDto>> GetApprovalRequestsAsync(string? status);
        Task<CategoryCodeApprovalDto?> GetApprovalRequestByIdAsync(int requestId);
        Task ApproveCategoryChangeAsync(int requestId);
        Task RejectCategoryChangeAsync(int requestId, string reason);
        Task<CategoryChangeValidationResult> ValidateAndCheckEligibilityAsync(int vendorId);
    }
}
