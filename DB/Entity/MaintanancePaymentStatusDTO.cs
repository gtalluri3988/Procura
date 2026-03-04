using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MaintanancePaymentStatusDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? PaymentStatus { get; set; }

        public string? Amount { get; set; }

        public int? ResidentId { get; set; }
       
        public long? PaymentId { get; set; }
        public string? FeeType { get; set; }
        public int? MaintananceMonth { get; set; }

        public int? MaintananceYear { get; set; }
        
        public PaymentRequest? PaymentRequest { get; set; }
    }
}
