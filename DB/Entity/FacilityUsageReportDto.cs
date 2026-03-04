using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class FacilityUsageReportDto
    {
        public string Month { get; set; }
        public string Facility { get; set; }
        public int TotalBookings { get; set; }
        public int UniqueResidents { get; set; }
        public string MostActiveDay { get; set; }
        public decimal RevenueCollected { get; set; }
        public decimal PendingPayment { get; set; }
    }
}
