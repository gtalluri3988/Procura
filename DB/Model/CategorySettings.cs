using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class CategorySettings
    {
        public int Id { get; set; }
        public int EditCategoryCodeAfterMonths { get; set; }
        public int EditCategoryCodeLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
