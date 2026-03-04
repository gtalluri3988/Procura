using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IAdminRepository
    {

        Task<IEnumerable<AdminDTO>> GetAllAsync();
        Task<AdminDTO> GetByIdAsync(int id);
        Task<AdminDTO> AddAsync(AdminDTO dto);
        Task UpdateAsync(int id, AdminDTO dto);
        Task DeleteAsync(int id);
    }
}
