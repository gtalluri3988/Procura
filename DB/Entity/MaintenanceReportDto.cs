using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class OutstandingMaintenanceReportDto
    {
        public string UnitNo { get; set; }
        public string ResidentName { get; set; }
        public decimal Amount { get; set; }
    }
}
