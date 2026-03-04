using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class JobScope:BaseEntity
    {
        public int Id { get; set; }

        // Job Category
        public string JobCategory { get; set; }      // Kontrak Pertanian

        public string ServiceCode { get; set; }      // KPTN001
        public string ShortText { get; set; }        // Membina pagar kawat berduri
        public string Unit { get; set; }             // M, HA

        public string DataType { get; set; }         // PO Kontrak Service
        public bool IsPoAccountAssigned { get; set; } // Y/N

        public string MaterialGroup { get; set; }    // DA280A
        public string RujukanIoWbs { get; set; }     // C883/C884
        public string GLAccount { get; set; }        // 53100040

        public decimal BudgetLimit { get; set; }     // 150000.00

        public string MaterialGroupDescription { get; set; }

        public bool IsActive { get; set; } = true;
    }

}
