using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class MaterialBudgetUploadDto
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public DateTime UploadDateTime { get; set; }

        public int UploadedBy { get; set; }

        public bool IsActive { get; set; }
    }
}
