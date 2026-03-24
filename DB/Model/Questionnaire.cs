using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public string Name { get; set; } // ESG Questionnaire
        public ICollection<Question> Questions { get; set; }
    }
}
