using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class RolePermissionRepository : RepositoryBase<RoleMenuPermission, RoleMenuPermissionDTO>, IRolePermissionRepository
    {
        public RolePermissionRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }

        public async Task UpdateRolePermissionAsync(int PermissionId, RoleMenuPermissionDTO rolePermission)
        {
            var updateRolePermission = await _context.RoleMenuPermissions
           .Where(x => x.RoleId == rolePermission.RoleId && x.SubMenuId == rolePermission.SubMenuId && x.MenuId == rolePermission.MenuId)
           .FirstOrDefaultAsync();
            if (updateRolePermission != null)
            {
                throw new Exception("Role permissions already added");
            }
            var entity = await _context.RoleMenuPermissions.FirstOrDefaultAsync(c => c.Id == PermissionId);
            if (entity != null)
            {
                entity.MenuId = rolePermission.MenuId;
                entity.SubMenuId = rolePermission.SubMenuId;
                entity.RoleId = rolePermission.RoleId;
                entity.UpdatedDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<RoleMenuPermissionDTO> GeteRolePermissionAsync(int PermissionId)
        {

            var entity = await _context.RoleMenuPermissions.FirstOrDefaultAsync(c => c.Id == PermissionId);
            return _mapper.Map<RoleMenuPermissionDTO>(entity);
        }

        public async Task<List<RoleMenuPermissionDTO>> GetAllMenuPermissionListAsync()
        {

            return await _context.RoleMenuPermissions
                .Select(c => new RoleMenuPermissionDTO
                {
                    Id = c.Id,
                    MenuId = c.MenuId ?? 0,
                    SubMenuId = c.SubMenuId ?? 0,
                    RoleId = c.RoleId ?? 0,
                    SubMenuName = _context.Menus.Where(m => m.Id == c.SubMenuId).Select(m => m.Name).FirstOrDefault(),
                    MenuName = c.Menu != null ? c.Menu.Name : null,
                    RoleName = c.Role != null ? c.Role.Name : null,

                })
                .ToListAsync();
        }
        public async Task<RoleMenuPermissionDTO> SaveMenuRolePermission(RoleMenuPermissionDTO roleMenuPermission)
        {
            var rolePermission = await _context.RoleMenuPermissions
            .Where(x => x.RoleId == roleMenuPermission.RoleId
            && x.SubMenuId == roleMenuPermission.SubMenuId && x.MenuId == roleMenuPermission.MenuId)
            .FirstOrDefaultAsync();
            if (rolePermission != null)
            {
                throw new Exception("Role permissions already added");
            }
            var entity = _mapper.Map<RoleMenuPermission>(roleMenuPermission);
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<RoleMenuPermissionDTO>(rolePermission);
        }

        public async Task<IEnumerable<MenuResponseDto>> GetRolesMenu(int roleId)
        {
            // Step 1: Fetch all parent menus
            var parentMenus = await _context.Menus
                .Where(m => m.ParentId == "0")
                .ToListAsync();

            // Step 2: Get RoleMenuPermissions for the role
            var rolePermissions = await _context.RoleMenuPermissions
                .Where(rmp => rmp.RoleId == roleId)
                .ToListAsync();

            // Step 3: Get all referenced submenus (if any)
            var submenuIds = rolePermissions
                .Where(rmp => rmp.SubMenuId != null)
                .Select(rmp => rmp.SubMenuId.Value)
                .Distinct()
                .ToList();

            var submenus = await _context.Menus
                .Where(m => submenuIds.Contains(m.Id))
                .ToListAsync();

            // Step 4: Assemble response in-memory
            var result = parentMenus
                .Where(pm => rolePermissions.Any(rp => rp.MenuId == pm.Id)) // only include if role has permission
                .Select(pm => new MenuResponseDto
                {
                    MenuId = pm.Id,
                    MenuName = pm.Name,
                    Url = pm.Url,
                    Submenus = rolePermissions
                        .Where(rp => rp.MenuId == pm.Id && rp.SubMenuId != null)
                        .Select(rp =>
                        {
                            var sm = submenus.FirstOrDefault(s => s.Id == rp.SubMenuId);
                            return sm == null ? null : new SubmenuDto
                            {
                                SubmenuId = sm.Id,
                                SubmenuName = sm.Name ?? "",
                                Url = sm.Url
                            };
                        })
                        .Where(sm => sm != null)
                        .ToList()
                })
                .ToList();

            return result;
        }

    }
}
