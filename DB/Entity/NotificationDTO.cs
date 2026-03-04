using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class NotificationDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public int ResidentId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
