using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MonthlyVisitorReportDto
    {
        public string Month { get; set; }          // Jul-25
        public int TotalVisitors { get; set; }
        public int UniqueVisitors { get; set; }
        public decimal AvgVisitorsPerDay { get; set; }
        public string PeakDay { get; set; }
    }

}
