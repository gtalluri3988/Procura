using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDTO>> GetAllAsync();

        Task<EventDTO> GetByIdAsync(int id);
        Task<EventDTO> AddAsync(EventDTO dto);
        Task UpdateAsync(int id, EventDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<EventDTO>> GetAllEventsForAdminAsync();
        Task<string> GenerateRunningNo(EventDTO dto);
        Task SendEventQREmail(QRImageModel model);
        Task<IEnumerable<EventDTO>> GetAllEventsForResidentAsync();
    }
}
