using DB.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IResidentUploadHistoryRepository
    {
        Task<IEnumerable<ResidentUploadHistoryDTO>> GetAllAsync();
        Task<ResidentUploadHistoryDTO> GetByIdAsync(int id);
        Task<ResidentUploadHistoryDTO> AddAsync(ResidentUploadHistoryDTO dto);
        Task UpdateAsync(int id, ResidentUploadHistoryDTO dto);
        Task DeleteAsync(int id);
        Task<string> UploadData(IFormFile file, string fileName, string attachment, string communityId, List<Dictionary<string, object>> rows);
        Task<IEnumerable<ResidentUploadHistoryDTO>> GetAllResidentUploadHistoryAsync();
    }
}
