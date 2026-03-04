using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class TenderSettings
    {
        public int Id { get; set; }
        public decimal MinCapitalRequiredPercent { get; set; }
        public int NegotiationLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
