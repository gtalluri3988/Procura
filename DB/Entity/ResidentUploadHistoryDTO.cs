using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ResidentUploadHistoryDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? FileName { get; set; }
        public string? Attachment { get; set; }
        public int UploadedBy { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public string? Name { get; set; }
        public string? CommunityName { get; set; }
    }
}
