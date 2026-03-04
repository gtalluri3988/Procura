using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class RoleService:IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task UpdateRoleAsync(int id, RoleDTO dto)
        {
            await _roleRepository.UpdateRoleAsync(id, dto);
        }
        public async Task SaveRoleAsync(RoleDTO dto)
        {
            await _roleRepository.AddAsync(dto);
        }
        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllAsync();
        }
    }
}
