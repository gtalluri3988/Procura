using DB.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IResidentUploadHistoryService
    {
        Task UpdateResidentUploadHistoryAsync(int id, ResidentUploadHistoryDTO dto);

        Task SaveResidentUploadHistoryAsync(ResidentUploadHistoryDTO dto);

        Task<ResidentUploadHistoryDTO> GetResidentUploadHistoryByIdAsync(int id);

        
        Task<string> UpdateDataAsync(IFormFile file, string fileName, string attachment,string communityId, List<Dictionary<string, object>> rows);
        Task<IEnumerable<ResidentUploadHistoryDTO>> GetAllResidentUploadHistoryAsync();
    }
}

