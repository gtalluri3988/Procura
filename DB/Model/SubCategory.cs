using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class SubCategory : BaseEntity
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

      
     

        public string SubCategoryName { get; set; }

        //public Category Category { get; set; }

        //public CodeMaster CodeMaster { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}
