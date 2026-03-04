using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO> GetByIdAsync(int id);
        Task<RoleDTO> AddAsync(RoleDTO dto);
        Task UpdateAsync(int id, RoleDTO dto);
        Task DeleteAsync(int id);
        Task UpdateRoleAsync(int roleId, RoleDTO role);
    }
}
