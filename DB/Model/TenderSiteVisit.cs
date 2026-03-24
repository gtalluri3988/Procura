using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderSiteVisit:BaseEntity
    {
        public int Id { get; set; }

        public int TenderId { get; set; }

        public bool SiteVisitRequired { get; set; }

        public DateTime VisitDate { get; set; }

        public string Venue { get; set; }

        public string Attendance { get; set; }

        public string Remarks { get; set; }

        public string FormFile { get; set; }
        [JsonIgnore]
        public TenderApplication? Tender { get; set; }
    }
}
