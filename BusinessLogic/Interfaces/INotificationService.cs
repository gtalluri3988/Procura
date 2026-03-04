using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync();
        Task<NotificationDTO> GetNotoficationByIdAsync(int id);
        Task<NotificationDTO> SaveNotificationAsync(NotificationDTO dto);
        Task UpdateNotificationsAsync(int id, NotificationDTO dto);
        Task<IEnumerable<NotificationDTO>> GetNotificationByResidentIdAsync(int id);
    }
}
