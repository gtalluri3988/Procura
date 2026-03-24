using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class QuestionAnswer:BaseEntity
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        public int QuestionId { get; set; }

        public string Answer { get; set; } // Yes / No

        public DateTime AnswerDate { get; set; }

        public Vendor Vendor { get; set; }

        public Question Question { get; set; }
    }
}
