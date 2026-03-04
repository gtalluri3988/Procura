namespace Api.Helpers
{
    using System.Security.Cryptography;
    using System.Text;

    public static class SignatureHelper
    {
        public static string GenerateSignature(string requestBody, string secretKey)
        {
            var signingPayload = requestBody + secretKey;
            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(signingPayload));
                var signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return signature;
            }
        }

       
            // signature = SHA512( requestBody + secretKey )
            public static string Sign(string requestBody, string secretKey)
            {
                var payload = requestBody + secretKey;
                using var sha = SHA512.Create();
                var bytes = Encoding.UTF8.GetBytes(payload);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            public static bool Verify(string requestBody, string secretKey, string signature)
            {
                var expected = Sign(requestBody, secretKey);
                return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
            }
        
    }

}
