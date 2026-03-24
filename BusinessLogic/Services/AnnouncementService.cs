using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Helper;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AnnouncementService : IAnnouncementService
    {

        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementService(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        public async Task DeleteAnnouncementAsync(int id)
        {
            await _announcementRepository.DeleteAnnouncementAsync(id);
        }

        public async Task<AnnouncementDto> GetAnnouncementByIdAsync(int id)
        {
            return await _announcementRepository.GetAnnouncementByIdAsync(id);
        }

        public async Task<List<AnnouncementDto>> GetAnnouncementsAsync(AnnouncementType type)
        {
            return await _announcementRepository.GetAnnouncementsAsync(type);
        }

        public async Task UpdateAnnouncementAsync(AnnouncementDto dto)
        {
            await _announcementRepository.UpdateAnnouncementAsync(dto);
        }

        public async Task<int> AddAnnouncementAsync(AnnouncementDto dto)
        {
            return await _announcementRepository.AddAnnouncementAsync(dto);
        }
    }
}
