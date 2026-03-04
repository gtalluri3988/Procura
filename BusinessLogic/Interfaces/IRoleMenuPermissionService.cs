using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IRoleMenuPermissionService
    {
        //Task<IEnumerable<RoleMenuPermissionDTO>> GetAllMenuRolesAsync();
        //Task<RoleMenuPermissionDTO> GetMenuRoleByIdAsync(int id);
        //Task<RoleMenuPermissionDTO> CreateMenuRoleAsync(RoleMenuPermissionDTO dto);
        //Task UpdateMenuRoleAsync(int id, RoleMenuPermissionDTO dto);
        //Task DeleteMenuRoleAsync(int id);
        Task<RoleMenuPermissionDTO> CreateMenuRoleAsync(RoleMenuPermissionDTO dto);
        Task<IEnumerable<RoleMenuPermissionDTO>> GetAllMenuRolesAsync();
        Task UpdateMenuRoleAsync(int id, RoleMenuPermissionDTO dto);
        Task<IEnumerable<MenuResponseDto>> GetAllMenusByRolesAsync(int roleId);
    }
}
