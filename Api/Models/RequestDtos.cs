namespace Api.Models
{
    public class ProductDto
    {
        public string name { get; set; }
        public string qty { get; set; }
        public string amount { get; set; }
    }

    public class TransactionRequestDto
    {
        public string? merchantId { get; set; }
        public string txType { get; set; } = "SALE";
        public string txAmount { get; set; } // "15.00"
        public string txCurrency { get; set; } = "MYR";
        public string txChannel { get; set; } // "CC", "EW", "DD"
        public string orderId { get; set; }
        public string orderRef { get; set; }
        public List<ProductDto> productList { get; set; }
        public string? custName { get; set; }
        public string? custEmail { get; set; }
        public string? custContact { get; set; }

        public int? ResidentId { get; set; }
        public int? VisitorId { get; set; } 
        public int? CommunityId { get; set; }
        public int? PaymentTypeId { get; set; }
    }

    public class TransactionQueryDto
    {
        public string merchantId { get; set; }
        public string txId { get; set; }
    }

    public class TransactionVoidDto
    {
        public string merchantId { get; set; }
        public string txId { get; set; }
        public string txAmount { get; set; }
        public string txCurrency { get; set; }
        public string remark { get; set; }
    }

    public class AmpersandResponseBase
    {
        public string ret { get; set; }
        public string msg { get; set; }
    }

}
