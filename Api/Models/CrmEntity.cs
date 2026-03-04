namespace Api.Models
{
    internal class CrmEntity
    {
        public string? TxId { get; set; }
        public string? OrderId { get; set; }
        public string? TxChannel { get; set; }
        public string? TxStatus { get; set; }
        public string? TxDt { get; set; }
    }

    public class SignatureRequest
    {
        public string? RequestBody { get; set; }
        public string? SecretKey { get; set; }
    }

    public class Product
    {
        public string? Name { get; set; }
        public int Qty { get; set; }
        public string? Amount { get; set; }
    }

    public class PaymentRequestModel
    {
        public string? MerchantId { get; set; }
        public string? TxType { get; set; }
        public string? TxAmount { get; set; }
        public string? TxCurrency { get; set; }
        public string? TxChannel { get; set; }
        public string? OrderId { get; set; }
        public string? OrderRef { get; set; }
        public List<Product>? ProductList { get; set; }
        public string? CustName { get; set; }
        public string? CustEmail { get; set; }
        public string? CustContact { get; set; }
        public string? RedirectUrl { get; set; }
        public string? WebhookUrl { get; set; }
    }

    public class PaymentResponseModel
    {
        public int Ret { get; set; }
        public string Msg { get; set; }
        public string? MerchantId { get; set; }
        public string? OrderId { get; set; }
        public string? TxId { get; set; }
        public string? CheckoutUrl { get; set; }
        public DateTime CheckoutDt { get; set; }
    }

    public class ForgotPassword
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public int ResidentId { get; set; }
    }
}