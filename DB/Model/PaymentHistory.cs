using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DB.EFModel
{
    public class PaymentHistory : BaseEntity
    {
        [Key]  // Marks as Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }
        [MaxLength(200)]
        public string? GroupID { get; set; }
        [MaxLength(200)]
        public string? TranID { get; set; }
        public DateTime? TransactionDateTime { get; set; }
        [MaxLength(100)]
        public string? OrderNo { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? PaymentTypeId { get; set; }
        [MaxLength(50)]
        public string? AuthCode { get; set; }
        [MaxLength(50)]
        public string? CardType { get; set; }
        [MaxLength(50)]
        public string? Bank { get; set; }
        [MaxLength(50)]
        public string? ErrorMessage { get; set; }
        [MaxLength(50)]
        public string? Amount { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public PaymentType? PaymentType { get; set; }
    }
}
