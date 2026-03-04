using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;

namespace BusinessLogic.Services
{
    public class RoleMenuPermissionService:IRoleMenuPermissionService
    {


        private readonly IRolePermissionRepository _roleMenuPermissionRepository;

        public RoleMenuPermissionService(IRolePermissionRepository roleMenuPermissionRepository)
        {
            _roleMenuPermissionRepository = roleMenuPermissionRepository;
        }


        public async Task<IEnumerable<RoleMenuPermissionDTO>> GetAllMenuRolesAsync()
        {
            return await _roleMenuPermissionRepository.GetAllMenuPermissionListAsync();
        }

        //public async Task<RoleMenuPermissionDTO> GetMenuRoleByIdAsync(int id)
        //{
        //    return await _roleMenuPermissionRepository.GetMenuRoleByIdAsync(id);
        //}

        public async Task<RoleMenuPermissionDTO> CreateMenuRoleAsync(RoleMenuPermissionDTO dto)
        {   
            return await _roleMenuPermissionRepository.SaveMenuRolePermission(dto);
        }

        public async Task UpdateMenuRoleAsync(int id, RoleMenuPermissionDTO dto)
        {
            await _roleMenuPermissionRepository.UpdateRolePermissionAsync(id, dto);
        }
        public async Task<IEnumerable<RoleMenuPermissionDTO>> GetAllMenusByUserAsync()
        {
            return await _roleMenuPermissionRepository.GetAllMenuPermissionListAsync();
        }

        public async Task<IEnumerable<MenuResponseDto>> GetAllMenusByRolesAsync(int roleId)
        {
            return await _roleMenuPermissionRepository.GetRolesMenu(roleId);
        }



    }
}
