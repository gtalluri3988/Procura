using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDTO>> GetAllMenusAsync();
        Task<MenuDTO> GetMenuByIdAsync(int id);
        Task<MenuDTO> CreateMenuAsync(MenuDTO dto);
        Task UpdateMenuAsync(int id, MenuDTO dto);
        Task<IEnumerable<MenuDTO>> GetSubMenuList(int menuId);
        //Task DeleteMenuAsync(int id);
    }
}
