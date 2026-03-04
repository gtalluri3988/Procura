using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class RoleMenuPermission:BaseEntity
    {
        public int Id { get; set; }
        public Nullable<int> ModuleId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> SubMenuId { get; set; }
        public Nullable<bool> IsEdit { get; set; }
        public Nullable<bool> IsView { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public bool Status { get; set; }

        public Role? Role { get; set; }
        public Menu? Menu { get; set; }

    }
}
