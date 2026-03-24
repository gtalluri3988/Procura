using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderCategoryCode:BaseEntity
    {
        public int Id { get; set; }

        public int TenderId { get; set; }

        public int CodeMasterId { get; set; }
        public CodeMaster? CodeMaster { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? SubCategoryId { get; set; }
        public SubCategory? SubCategory { get; set; }

        public int? ActivityId { get; set; }
        public Activity? Activity { get; set; }
        [JsonIgnore]
        public TenderApplication? Tender { get; set; }
    }
}
