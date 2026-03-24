using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Question:BaseEntity
    {
        public int Id { get; set; }

        public int QuestionnaireId { get; set; }

        public string QuestionText { get; set; }

        public string? QuestionType { get; set; } // YesNo, Text, Dropdown

        public int? OrderNo { get; set; }

        public Questionnaire Questionnaire { get; set; }

        public ICollection<QuestionAnswer> Answers { get; set; }
    }
}
