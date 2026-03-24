using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class QuestionAnswerDto
    {

        public int Id { get; set; }

        public int VendorId { get; set; }

        public int QuestionId { get; set; }

        public string Answer { get; set; } // Yes / No

        public DateTime AnswerDate { get; set; }
        [JsonIgnore]
        public Vendor? Vendor { get; set; }
        [JsonIgnore]
        public Question? Question { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
    }
}
