using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class VendorPaymentStatus:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public string? PaymentStatus { get; set; }
        [ForeignKey(nameof(Vendor))]
        public int? VendorId { get; set; }
        [ForeignKey(nameof(PaymentRequest))]
        public long PaymentId { get; set; }
        public int? PaymentTypeId { get; set; }
        public Vendor? Vendor { get; set; }
        public PaymentRequest? PaymentRequest { get; set; }

        public PaymentType? PaymentType { get; set; }
    }
}
