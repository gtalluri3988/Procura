using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IContentService
    {
        Task<IEnumerable<ContentManagementDTO>> GetAllContentsAsync();
        Task<ContentManagementDTO> GetContentByIdAsync(int id);
        Task<ContentManagementDTO> CreateContentAsync(ContentManagementDTO dto);
        Task UpdateContentAsync(int id, ContentManagementDTO dto);
        Task DeleteContentAsync(int id);
    }
}
