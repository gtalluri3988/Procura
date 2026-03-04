using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class OutstandingReportSummaryDto
    {
        public List<OutstandingMaintenanceReportDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
