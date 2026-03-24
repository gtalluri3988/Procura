using DB.Entity;
using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementDto>> GetAnnouncementsAsync(AnnouncementType type);
        Task<AnnouncementDto> GetAnnouncementByIdAsync(int id);
        Task UpdateAnnouncementAsync(AnnouncementDto dto);
        Task DeleteAnnouncementAsync(int id);
        Task<int> AddAnnouncementAsync(AnnouncementDto dto);
    }
}
