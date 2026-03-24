using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderIssuenceApproval:BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int ReviewLevel { get; set; } // 1st, 2nd
        public int ReviewerId { get; set; }
        public int? TenderStatusId { get; set; }
        public string? Remarks { get; set; }
        public DateTime ReviewDateTime { get; set; }
        public TenderApplication? Tender { get; set; }
        public TenderApplicationStatus? TenderStatus { get; set; }
        public User? Reviewer { get; set; }
    }
}
