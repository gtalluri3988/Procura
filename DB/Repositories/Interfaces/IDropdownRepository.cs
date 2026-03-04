using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IDropdownRepository
    {
        Task<List<DropdownItem>> GetDropdownDataAsync(string dropdownType);
    }
}
