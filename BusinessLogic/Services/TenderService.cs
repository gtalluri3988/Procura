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
    }
}
