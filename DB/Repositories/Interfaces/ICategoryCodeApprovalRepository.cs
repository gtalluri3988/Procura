using DB.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface ICategoryCodeApprovalRepository
    {
        Task<int> CreateApprovalRequestAsync(int vendorId, string requestType, string requestedDataJson, List<CategoryCodeApprovalItemDto> items);
        Task<List<CategoryCodeApprovalListDto>> GetApprovalRequestsAsync(string? status);
        Task<CategoryCodeApprovalDto?> GetApprovalRequestByIdAsync(int requestId);
        Task<bool> HasPendingRequestAsync(int vendorId);
        Task ApproveRequestAsync(int requestId);
        Task RejectRequestAsync(int requestId, string reason);
        Task<int> GetPendingRequestCountAsync(int vendorId);
    }
}
