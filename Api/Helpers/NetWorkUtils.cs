namespace Api.Helpers
{

    public static class NetWorkUtils
    {
        public static string GetClientIp(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers["CF-CONNECTING-IP"];
            if (header.Count > 0)
            {
                return header[0];
            }
            header= httpContext.Request.Headers["HTTP_X_FORWARDED_FOR"];
            string ip = string.Empty;
            if (header.Count > 0)
            {
                return header[0];
            }
            if (!string.IsNullOrEmpty(ip))
            {
                var address=ip.Split(',');
                if(address.Length!=0)
                    return address[0];
            }
            // Get direct remote IP
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
