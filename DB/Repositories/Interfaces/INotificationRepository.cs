using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<NotificationDTO>> GetAllAsync();
        Task<NotificationDTO> GetByIdAsync(int id);
        Task<NotificationDTO> AddAsync(NotificationDTO dto);
        Task UpdateAsync(int id, NotificationDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<NotificationDTO>> GetNotificationByResidentIdAsync(int residentId);
    }
}
