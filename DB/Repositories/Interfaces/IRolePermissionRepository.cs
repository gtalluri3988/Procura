using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RoleMenuPermissionDTO>> GetAllAsync();
        Task<RoleMenuPermissionDTO> GetByIdAsync(int id);
        Task<RoleMenuPermissionDTO> AddAsync(RoleMenuPermissionDTO dto);
        Task UpdateAsync(int id, RoleMenuPermissionDTO dto);
        Task DeleteAsync(int id);
        Task<List<RoleMenuPermissionDTO>> GetAllMenuPermissionListAsync();
        Task<RoleMenuPermissionDTO> SaveMenuRolePermission(RoleMenuPermissionDTO roleMenuPermission);
        Task UpdateRolePermissionAsync(int PermissionId, RoleMenuPermissionDTO rolePermission);
        //Task<IEnumerable<RoleMenuPermissionDTO>> GetAllMenuRolesAsync();
        //Task<RoleMenuPermissionDTO> GetMenuRoleByIdAsync(int id);
        //Task<RoleMenuPermissionDTO> CreateMenuRoleAsync(RoleMenuPermissionDTO dto);
        //Task UpdateMenuRoleAsync(int id, RoleMenuPermissionDTO dto);
        //Task DeleteMenuRoleAsync(int id);
        Task<IEnumerable<MenuResponseDto>> GetRolesMenu(int roleId);
    }
}
