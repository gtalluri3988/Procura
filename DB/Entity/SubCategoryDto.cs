using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class SubCategoryDto
    {
        public int? Id { get; set; }

        public int CategoryId { get; set; }

        public int CodeId { get; set; }

        public string SubCategoryCode { get; set; }

        public string SubCategoryName { get; set; }

        public List<ActivityDto> Activities { get; set; }
    }
}
