using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IMenuRepository
    {
        Task<IEnumerable<MenuDTO>> GetAllAsync();
        Task<MenuDTO> GetByIdAsync(int id);
        Task<MenuDTO> AddAsync(MenuDTO dto);
        Task UpdateAsync(int id, MenuDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<MenuDTO>> GetSubMenuList(int menuId);




    }
}
