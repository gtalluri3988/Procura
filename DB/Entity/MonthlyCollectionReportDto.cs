using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MonthlyCollectionReportDto
    {
        public string Month { get; set; }
        public decimal MaintenanceCollected { get; set; }
        public decimal SinkingFundCollected { get; set; }
        public decimal OtherFees { get; set; }
        public decimal Total => MaintenanceCollected + SinkingFundCollected + OtherFees;
    }
}
