using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Announcement:BaseEntity
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Reference { get; set; }

        public AnnouncementType Type { get; set; } // News / Advertisement / AwardResult

        public DateTime? Date { get; set; }

        public DateTime? ClosingDate { get; set; }
       
        public int? VendorId { get; set; }

        public decimal? Value { get; set; }

        public string Description { get; set; }

        public Vendor? Vendor { get; set; }


    }
}
