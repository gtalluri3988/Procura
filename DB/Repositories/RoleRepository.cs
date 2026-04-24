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
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class RoleRepository : RepositoryBase<Role, RoleDTO>, IRoleRepository
    {
        public RoleRepository(ProcuraDbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }

        public async Task UpdateRoleAsync(int roleId, RoleDTO role)
        {
            var entity = await _context.Roles.FirstOrDefaultAsync(c => c.Id == roleId);
            if (entity != null && role!=null)
            {
                entity.Name = role.Name??"";
                entity.Status = role.Status;
                entity.UpdatedDate = DateTime.Now;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            { }
        }

        public async Task<bool> IsRoleAssignedToAnyUserAsync(int roleId)
        {
            return await _context.Users.AnyAsync(u => u.RoleId == roleId);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var entity = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (entity == null) return false;

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
