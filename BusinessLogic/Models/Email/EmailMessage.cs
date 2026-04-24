namespace BusinessLogic.Models.Email
{
    public class EmailMessage
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }
}
