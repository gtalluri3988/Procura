using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class RoleMenuPermissionDTO
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public Nullable<int> SubMenuId { get; set; }
        public Nullable<bool> IsEdit { get; set; }
        public Nullable<bool> IsView { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public bool Status { get; set; }
        public string? RoleName { get; set; }
        public string? MenuName { get; set; }
        public string? SubMenuName { get; set; }
    }
}
