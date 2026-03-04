using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class CompanyEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CompanyCategoryId { get; set; }
        [JsonIgnore]
        public CompanyCategory CompanyCategory { get; set; }
    }
}
