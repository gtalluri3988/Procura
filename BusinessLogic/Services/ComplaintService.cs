using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Services
{
    public class ComplaintService : IComplaintService
    {

        private readonly IComplaintRepository _complaintRepository;

        public ComplaintService(IComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintAsync()
        {
            return await _complaintRepository.GetAllComplaintsAsync();
        }

        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintForResidentAsync()
        {
            return await _complaintRepository.GetAllComplaintsForResidentAsync();
        }

        public async Task<ComplaintDTO> GetComplaintByIdAsync(int id)
        {
            return await _complaintRepository.GetComplaintByComplaintIdAsync(id);
        }

        public async Task<ComplaintDTO> CreateComplaintAsync(ComplaintDTO dto)
        {
            return await _complaintRepository.CreateComplaintAsync(dto);
        }

        public async Task UpdateComplaintAsync(int id, ComplaintDTO dto, List<IFormFile> photos)
        {
            await _complaintRepository.UpdateComplaintAsync(id, dto, photos);
        }

        public async Task DeleteComplaintAsync(int id)
        {
            await _complaintRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintsByCommunity(int communityId)
        {
            return await _complaintRepository.GetAllComplaintsByCommunityAsync(communityId);
        }

        public async Task<IEnumerable<ComplaintDTO>> SearchComplaintBySearchParamsAsync(ComplaintDTO searchModel)
        {
            return await _complaintRepository.SearchComplaintBySearchParamsAsync(searchModel);
        }

        public async Task SubmitComplaintAsync(int complaintId)
        {
            await _complaintRepository.SubmitComplaintAsync(complaintId);
        }
    }
}
