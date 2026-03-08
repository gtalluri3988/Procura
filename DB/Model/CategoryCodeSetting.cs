using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class CategoryCodeSetting:BaseEntity
    {
        public int Id { get; set; }

        // Registration
        public int EditCategoryCodeAfterMonth { get; set; }          // RM
        public int EditCategoryCodeLimitAfterYear { get; set; }
    }
}
