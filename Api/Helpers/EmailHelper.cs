namespace Api.Helpers
{
    public class EmailHelper
    {



        public static string GetWelcomeEmailBody(
    string residentFullName,
    string residentEmail,
    string randomPassword,
    string residentPageUrl)
        {
            return $@"
        <html>
        <body>
            <p>Dear {residentFullName},</p>

            <p>You have been successfully registered in our system by the administrator.</p>

            <p><strong>Please find your login details below:</strong></p>

            <p>
                System URL: <a href='{residentPageUrl}'>{residentPageUrl}</a><br/>
                Username: {residentEmail}<br/>
                Temporary Password: {randomPassword}
            </p>

            <p>
                For security reasons, you will be prompted to change your password upon your first login.
            </p>

            <p>If you have any questions or encounter any issues, please contact the system administrator.</p>

            <p>Best regards,<br/>CSA Team</p>
        </body>
        </html>";
        }


        public static string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@$?";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
