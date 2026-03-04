using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MenuResponseDto
    {
        public int MenuId { get; set; }
        public string? MenuName { get; set; }
        public string? Url { get; set; }
        public List<SubmenuDto> Submenus { get; set; } = new List<SubmenuDto>();
    }

    public class SubmenuDto
    {
        public int SubmenuId { get; set; }
        public string? SubmenuName { get; set; }

        public string? Url { get; set; }
    }
}
