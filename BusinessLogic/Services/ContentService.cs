using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ContentService : IContentService
    {

        private readonly IContentRepository _contentRepository;

        public ContentService(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<IEnumerable<ContentManagementDTO>> GetAllContentsAsync()
        {
            return await _contentRepository.GetAllContentsAsync();
        }

        public async Task<ContentManagementDTO> GetContentByIdAsync(int id)
        {
            return await _contentRepository.GetAllContentByIdAsync(id);
        }

        public async Task<ContentManagementDTO> CreateContentAsync(ContentManagementDTO dto)
        {
            return await _contentRepository.CreateContentAsync(dto);
        }

        public async Task UpdateContentAsync(int id, ContentManagementDTO dto)
        {
            await _contentRepository.UpdateContentAsync(id, dto);
        }

        public async Task DeleteContentAsync(int id)
        {
            await _contentRepository.DeleteAsync(id);
        }

        
    }
}
