using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CategoryDto
    {
        public int? Id { get; set; }

        public int CodeMasterId { get; set; }

       

        public string CategoryName { get; set; }

       

        public List<SubCategoryDto>? SubCategories { get; set; }
    }
}
