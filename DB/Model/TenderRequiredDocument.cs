using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderRequiredDocument: BaseEntity
    {
        public int Id { get; set; }

        public int TenderApplicationId { get; set; }

        public string DocumentName { get; set; }

        public string Requirement { get; set; }

        public string Submission { get; set; }

        public DateTime CreatedDate { get; set; }

        public TenderApplication TenderApplication { get; set; }
    }
}
