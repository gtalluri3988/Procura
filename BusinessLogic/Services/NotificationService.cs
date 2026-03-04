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
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync()
        {
            return await _notificationRepository.GetAllAsync();
        }

        public async Task<NotificationDTO> GetNotoficationByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<NotificationDTO> SaveNotificationAsync(NotificationDTO dto)
        {
            return await _notificationRepository.AddAsync(dto);
        }

        public async Task UpdateNotificationsAsync(int id, NotificationDTO dto)
        {
            await _notificationRepository.UpdateAsync(id, dto);

        }

        public async Task<IEnumerable<NotificationDTO>> GetNotificationByResidentIdAsync(int id)
        {
            return await _notificationRepository.GetNotificationByResidentIdAsync(id);
        }
    }
}
