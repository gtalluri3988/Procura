using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderReview:BaseEntity
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }
        public int ReviewLevel { get; set; }
        public int ReviewedByUserId { get; set; }
        public int TenderApplicationStatusId { get; set; }
        public string? Remarks { get; set; }
        [JsonIgnore]
        public TenderApplicationStatus? TenderApplicationStatus { get; set; }
        ////[JsonIgnore]
        //[ForeignKey(nameof(ReviewedByUserId))]
        //public User? ReviewedByUser { get; set; }
        public User? ReviewedByUser { get; set; }
        public DateTime ReviewDate { get; set; }
        [JsonIgnore]
        public TenderApplication TenderApplication { get; set; }
    }
}
