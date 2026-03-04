using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class PaymentChannel
    {
        public int Id { get; set; }  // Primary Key
        public string? Name { get; set; }
    }
}
