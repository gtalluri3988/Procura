using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Activity : BaseEntity
    {
        public int Id { get; set; }

        public int SubCategoryId { get; set; }

      
        public string? ActivityCode { get; set; }

        public string ActivityName { get; set; }

        public SubCategory SubCategory { get; set; }

        //public CodeMaster CodeMaster { get; set; }
    }
}
