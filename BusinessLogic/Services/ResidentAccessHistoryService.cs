using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ResidentAccessHistoryService : IResidentAccessHistoryService
    {
        private readonly IResidentAccessHistoryRepository _residentAccessHistoryRepository;

        public ResidentAccessHistoryService(IResidentAccessHistoryRepository residentAccessHistoryRepository)
        {
            _residentAccessHistoryRepository = residentAccessHistoryRepository;

        }
        

        public async Task<IEnumerable<ResidentAccessHistoryDTO>> GetAllResidentAccessHistoryAsync(int? communityId,bool isCSAAdmin)
        {
            return await _residentAccessHistoryRepository.GetAllResidentAccessHistoryAsync(communityId, isCSAAdmin);
        }

        Task<ResidentAccessHistoryDTO> IResidentAccessHistoryService.GetResidentAccessHistoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

       
        Task IResidentAccessHistoryService.UpdateResidentAccessHistoryAsync(int id, ResidentAccessHistoryDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResidentAccessHistoryDTO> SaveResidentAccessHistoryAsync(ResidentAccessHistoryDTO resident)
        {
            return await _residentAccessHistoryRepository.SaveResidentAccessHistoryAsync(resident);
        }

        public async Task<ResidentAccessHistoryDTO> GetResidentAccessHistoryByIdAsync(int? AccessId)
        {
            return await _residentAccessHistoryRepository.GetResidentAccessHistoryByIdAsync(AccessId);
        }

        public async Task<IEnumerable<ResidentAccessHistoryDTO>> SearchResidentAccessHistoryBySearchParamsAsync(ResidentAccessHistoryDTO searchModel)
        {
            return await _residentAccessHistoryRepository.SearchResidentAccessHistoryBySearchParamsAsync(searchModel);
        }
    }
}
