using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class TenderDocumentDto
    {
        public int JobCategoryId { get; set; }


        public string DocumentName { get; set; }

        public string Requirement { get; set; }
        [JsonIgnore]
        public JobCategory? JobCategory { get; set; }



    }
}
