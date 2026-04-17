using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class AnnouncementDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Reference { get; set; }

        public AnnouncementType Type { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? ClosingDate { get; set; }

        public int? VendorId { get; set; }

        public decimal? Value { get; set; }

        public string Description { get; set; }

        public bool? Status { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
