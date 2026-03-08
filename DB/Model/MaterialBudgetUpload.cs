using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class MaterialBudgetUpload:BaseEntity
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string? FilePath { get; set; }

        public DateTime UploadDateTime { get; set; }

        public bool IsActive { get; set; }
    }
}
