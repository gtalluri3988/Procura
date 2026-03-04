using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IRoleService
    {
        Task UpdateRoleAsync(int id, RoleDTO dto);

        Task SaveRoleAsync(RoleDTO dto);

        Task<RoleDTO> GetRoleByIdAsync(int id);

        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
    }
       
}
