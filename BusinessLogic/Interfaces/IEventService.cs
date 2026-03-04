using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEventsAsync();
        Task<EventDTO> GetEventByIdAsync(int id);
        Task<EventDTO> SaveEventAsync(EventDTO dto);
        Task UpdateEventAsync(int id, EventDTO dto);

        Task<IEnumerable<EventDTO>> GetAllEventsForAdminAsync();
        Task SendEventQREmail(QRImageModel model);
        Task<IEnumerable<EventDTO>> GetAllEventsForResidentAsync();
    }
}
