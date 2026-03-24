using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DB.EFModel
{
    public class PaymentRequest:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public int? VendorId { get; set; }
        public int? PaymentTypeId { get; set; }

        public int? PaymentChannelId { get; set; }
        public string? OrderId { get; set; }

        public string? ROCNumber { get; set; }
        public string? TID { get; set; }
        public string? Invoice { get; set; }
        public string? InvoiceDesc { get; set; }
        public string? Amount { get; set; }
        public string? Email { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ApiOperation { get; set; }
        public string? CardType { get; set; }
        public string? ApiKey { get; set; }
        public string? CheckSum { get; set; }
        public PaymentHistory? PaymentHistory { get; set; }
        public Vendor? Vendor { get; set; }

        public PaymentChannel? PaymentChannel { get; set; }


    }
}
