using DB.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IComplaintService
    {
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintAsync();
        Task<ComplaintDTO> GetComplaintByIdAsync(int id);
        Task<ComplaintDTO> CreateComplaintAsync(ComplaintDTO dto);
        Task UpdateComplaintAsync(int id, ComplaintDTO dto, List<IFormFile> photos);
        Task DeleteComplaintAsync(int id);
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintsByCommunity(int communityId);
        Task<IEnumerable<ComplaintDTO>> SearchComplaintBySearchParamsAsync(ComplaintDTO searchModel);

        Task SubmitComplaintAsync(int complaintId);
        Task<IEnumerable<ComplaintDTO>> GetAllComplaintForResidentAsync();

    }
}
