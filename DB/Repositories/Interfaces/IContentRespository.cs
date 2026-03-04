using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<IEnumerable<ContentManagementDTO>> GetAllAsync();
        Task<ContentManagementDTO> GetByIdAsync(int id);
        Task<ContentManagementDTO> AddAsync(ContentManagementDTO dto);
        Task UpdateAsync(int id, ContentManagementDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ContentManagementDTO>> GetAllContentsAsync();
        Task<ContentManagementDTO> CreateContentAsync(ContentManagementDTO dto);
        Task<ContentManagementDTO> GetAllContentByIdAsync(int id);
        Task UpdateContentAsync(int facilityId, ContentManagementDTO facility);
        //Task UpdateFacilityAsync(int facilityId, FacilityDTO facility);
    }
}
