using DB.EFModel;
using DB.Helper;
using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class VendorCategoryDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }

        public int CodeMasterId { get; set; }
        [JsonIgnore]
        public CodeMaster? CodeMaster { get; set; }

        public int? CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }

        public int? SubCategoryId { get; set; }
        [JsonIgnore]
        public SubCategory? SubCategory { get; set; }

        public int? ActivityId { get; set; }
        [JsonIgnore]
        public Activity? Activity { get; set; }


    


    }

    public class VendorCategoryRequest
    {
        public List<VendorCategoryDto>? VendorCategoryDto { get; set; }
        public List<VendorCategoryCertificateDto>? VendorCategoryCertificateDto { get; set; }

    }

}
