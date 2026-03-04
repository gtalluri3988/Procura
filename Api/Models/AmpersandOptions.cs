namespace Api.Models
{
    public class AmpersandOptions
    {
        public string MerchantId { get; set; }
        public string SecretKey { get; set; }
        public bool UseSandbox { get; set; } = true;
        public string SandboxBaseUrl { get; set; }
        public string ProductionBaseUrl { get; set; }
        public string BaseUrl => UseSandbox ? SandboxBaseUrl : ProductionBaseUrl;
    }
}
