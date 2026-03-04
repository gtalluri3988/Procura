using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class PaymentDetailsDTO
    {
        public string? PaymentDescription { get; set; }
        public double? Amount { get; set; }
        public string Email { get; set; }
    }
}
