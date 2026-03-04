using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CardDTO
    {
        public int Id { get; set; }  // Primary Key
        public int? ResidentId { get; set; }
        public string? CardNo { get; set; }
        public int? StatusId { get; set; }
        public DateTime AssignDatetime { get; set; }

    }
}
