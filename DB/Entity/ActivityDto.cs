using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ActivityDto
    {
        public int? Id { get; set; }

        public int SubCategoryId { get; set; }

        public int CodeId { get; set; }

        public string ActivityCode { get; set; }

        public string ActivityName { get; set; }
    }
}
