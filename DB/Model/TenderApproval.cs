using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderApproval:BaseEntity
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }

        public int ApprovalLevel { get; set; }

        public string Status { get; set; }
        public string? Remarks { get; set; }
        
        public int ApprovedByUserId { get; set; }
        public int TenderApplicationStatusId { get; set; }
        [JsonIgnore]
        public TenderApplicationStatus? TenderApplicationStatus { get; set; }
        //[JsonIgnore]
        //[ForeignKey(nameof(ApprovedByUserId))]
        //public User? ApprovedByUser { get; set; }

        public DateTime ReviewDate { get; set; }
        [JsonIgnore]
        public TenderApplication TenderApplication { get; set; }


        
        public User? ApprovedByUser { get; set; }
    }
}
