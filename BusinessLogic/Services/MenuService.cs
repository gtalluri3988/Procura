using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class MenuService:IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuDTO>> GetAllMenusAsync()
        {
            return await _menuRepository.GetAllAsync();
        }

        public async Task<MenuDTO> GetMenuByIdAsync(int id)
        {
            return await _menuRepository.GetByIdAsync(id);
        }

        public async Task<MenuDTO> CreateMenuAsync(MenuDTO dto)
        {
            return await _menuRepository.AddAsync(dto);
        }

        public async Task UpdateMenuAsync(int id, MenuDTO dto)
        {
            await _menuRepository.UpdateAsync(id, dto);
        }

        //public async Task DeleteFacilityAsync(int id)
        //{
        //    await _menuRepository.DeleteMenuAsync(id);
        //}

        public async Task<IEnumerable<MenuDTO>> GetAllSubMenusByIdAsync(int menuId)
        {
            return await _menuRepository.GetSubMenuList(menuId);
        }

        public async Task<IEnumerable<MenuDTO>> GetSubMenuList(int menuId)
        {
            return await _menuRepository.GetSubMenuList(menuId);
        }
    }
}
