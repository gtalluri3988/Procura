using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderEvaluationCommittee:BaseEntity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int UserId { get; set; }
        public TenderApplication? Tender { get; set; }
        public User? User { get; set; }
    }
}
