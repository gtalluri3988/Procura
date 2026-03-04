using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class ErrorMessage
    {
        public ErrorMessage() { }
        public ErrorMessage(string message)
        { 
             this.message = message;
        }
        [Required]
        public string message { get; set; }
    }
}
