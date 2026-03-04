using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ResidentUploadHistoryService : IResidentUploadHistoryService
    {
        private readonly IResidentUploadHistoryRepository _residentUploadHistoryRepository;

        public ResidentUploadHistoryService(IResidentUploadHistoryRepository residentUploadHistoryRepository)
        {
            _residentUploadHistoryRepository = residentUploadHistoryRepository;

        }
        public async Task<IEnumerable<ResidentUploadHistoryDTO>> GetAllResidentUploadHistoryAsync()
        {
            return await _residentUploadHistoryRepository.GetAllResidentUploadHistoryAsync();
        }
        

        Task<ResidentUploadHistoryDTO> IResidentUploadHistoryService.GetResidentUploadHistoryByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task IResidentUploadHistoryService.SaveResidentUploadHistoryAsync(ResidentUploadHistoryDTO dto)
        {
            throw new NotImplementedException();
        }

        Task IResidentUploadHistoryService.UpdateResidentUploadHistoryAsync(int id, ResidentUploadHistoryDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateDataAsync(IFormFile file, string fileName, string attachment, string communityId, List<Dictionary<string, object>> rows)
        {
            return await _residentUploadHistoryRepository.UploadData(file,fileName,attachment,  communityId,rows);
        }
    }
}


