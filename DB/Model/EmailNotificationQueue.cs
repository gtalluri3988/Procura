using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class EmailNotificationQueue : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string RecipientEmail { get; set; }

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public bool IsHtml { get; set; } = true;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        public int RetryCount { get; set; } = 0;

        [Required]
        public int MaxRetries { get; set; } = 3;

        public DateTime? LastAttemptAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }

        public string ErrorMessage { get; set; }
    }
}
