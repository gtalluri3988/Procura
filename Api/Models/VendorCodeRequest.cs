namespace Procura.Models
{
    public class VendorCodeRequest
    {
        public int VendorId { get; set; }
        public string AccountNo { get; set; }

        public string GR_InvoiceVerification { get; set; }

        public string PaymentMethod { get; set; }
    }

    public class VendorDetailsChangeRequest
    {
        public int VendorId { get; set; }
       
    }
}
