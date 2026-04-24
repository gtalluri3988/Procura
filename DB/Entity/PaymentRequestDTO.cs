using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    
        public class PaymentRequestDTO
        {
        public long Id { get; set; }
        public string? merchantId { get; set; }
        public string? ROCNumber { get; set; }
        public string txType { get; set; } = "SALE";
            public string txAmount { get; set; } // "15.00"
            public string txCurrency { get; set; } = "MYR";
            public string txChannel { get; set; } // "CC", "EW", "DD"
            public string orderId { get; set; }
            public string orderRef { get; set; }

        public string? productName { get; set; }
        public string? custName { get; set; }
            public string? custEmail { get; set; }
            public string? custContact { get; set; }
            public int? VendorId { get; set; }
            
            public int? PaymentTypeId { get; set; }
        }
    
}
