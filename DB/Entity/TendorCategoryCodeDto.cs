using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class TendorCategoryCodeDto
    {
        public int Id { get; set; }
        public int? TenderId { get; set; }
        public int? CodeMasterId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? ActivityId { get; set; }
      
    }
}
