using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DB.EFModel
{
    public class PaymentResponse:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public int? PaymentID { get; set; }
        public string? TID { get; set; }
        public string? InvoiceNo { get; set; }
        public string? OrderDesc { get; set; }
        public string? PaymentDateTime { get; set; }
        public string? Currency { get; set; }
        public string? Amount { get; set; }
        public string? Result { get; set; }
        public string? ResponseStatus { get; set; }
        public string? ProcessorTransactionID { get; set; }
        public string? ProcessorMessage { get; set; }
        public string? Bank { get; set; }
        public string? PaymentMethod { get; set; }
        public string? AuthCode { get; set; }
        public string? CardType { get; set; }
        public string? Checksum { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorDesc { get; set; }
        public PaymentHistory? PaymentHistory { get; set; }
    }
}
