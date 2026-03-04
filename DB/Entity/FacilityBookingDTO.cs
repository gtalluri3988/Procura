using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class FacilityBookingDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? Amount { get; set; }
        public string? Deposit { get; set; }
        public int? PaymentId { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ResidentId { get; set; }
        public int? FacilityId { get; set; }
        public int? LotQuantity { get; set; }
        public string? RentalStartMonth { get; set; }
        public string? RentalStartYear { get; set; }
        public bool IsDepositRefund { get; set; }
        public string? RefundAmount { get; set; }
        public DateTime? RefundDateTime { get; set; }
        
    }
}
