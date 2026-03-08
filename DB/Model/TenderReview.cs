using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderReview:BaseEntity
    {
        public int Id { get; set; }
        public int TenderApplicationId { get; set; }

        public int ReviewLevel { get; set; }

        public string PicName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string MobileNo { get; set; }

        public string Status { get; set; }
        public string Remarks { get; set; }

        public DateTime ReviewDate { get; set; }

        public TenderApplication TenderApplication { get; set; }
    }
}
