using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class PaymentDTO
    {
        public int Id { get; set; }  // Primary Key
        public int ResidentId { get; set; }  // Primary Key
        public int PaymentTypeId { get; set; }
        public int PaymentStatusId { get; set; }
        public string? PaymentRef { get; set; }
        public double Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        
        public PaymentType? PaymentType { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
