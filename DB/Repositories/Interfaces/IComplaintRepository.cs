using DB.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IComplaintRepository
    {
        Task<IEnumerable<ComplaintDTO>> GetAllAsync();
        Task<ComplaintDTO> GetByIdAsync(int id);
        Task<ComplaintDTO> AddAsync(ComplaintDTO dto);
        Task UpdateAsync(int id, ComplaintDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintsAsync();
        Task<ComplaintDTO> GetComplaintByComplaintIdAsync(int complaintId);
        Task UpdateComplaintAsync(int complaintId, ComplaintDTO complaint, List<IFormFile> photos);
        Task<ComplaintDTO> CreateComplaintAsync(ComplaintDTO dto);
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintsByCommunityAsync(int communityId);
        Task<IEnumerable<ComplaintDTO>> SearchComplaintBySearchParamsAsync(ComplaintDTO searchModel);
        Task SubmitComplaintAsync(int complaintId);
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintsForResidentAsync();
    }
}
