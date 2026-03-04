using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }
        public decimal Price { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property (Many Products belong to One Category)
        public Category? Category { get; set; }
    }
}
