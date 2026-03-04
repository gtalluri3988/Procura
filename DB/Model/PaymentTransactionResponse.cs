using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class PaymentTransactionResponse:BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Ret { get; set; }
        public string? Msg { get; set; }
        [ForeignKey(nameof(PaymentRequest))]
        public long PaymentId { get; set; }
        public long? MerchantId { get; set; }
        public string OrderId { get; set; }
        public string? TxId { get; set; }

        public string? TxType { get; set; }
        public string? TxStatus { get; set; }

        public DateTime TxDt { get; set; }

        public string? TxAmount { get; set; }
        public string? TxCurrency { get; set; }
        public string? TxChannel { get; set; }

        public string? RespCode { get; set; }
        public string? SchemeId { get; set; }

        public PaymentRequest? PaymentRequest { get; set; }
    }

}
