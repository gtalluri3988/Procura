namespace Api.Controllers
{
    internal class GatewayParameter
    {
        internal string checksum;

        public string apiKey { get; set; }
        public string TID { get; set; }
        public string orderNo { get; set; }
        public string orderDescription { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public string method { get; set; }
        public string apiOperation { get; set; }
        public string cardType { get; set; }
        public string email { get; set; }
    }
}