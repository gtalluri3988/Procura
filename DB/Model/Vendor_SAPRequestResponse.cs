using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class Vendor_SAPRequestResponse:BaseEntity
    {
       
        public int Id { get; set; }  // Primary Key
        public int VendorId { get; set; }  // Primary Key
        public string? Request { get; set; }
        public string? Response { get; set; }
        public DateTime ResponseDateTime { get; set; }
    }
}
