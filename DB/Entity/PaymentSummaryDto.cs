using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class PaymentSummaryDto
    {
        public long PaymentRequestId { get; set; }
        public string? Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }

        public string? Invoice { get; set; }
    }

}
