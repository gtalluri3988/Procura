using System;

namespace DB.EFModel
{
    public class VendorCategoryChangeLog : BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangeDescription { get; set; } = string.Empty;
        public Vendor? Vendor { get; set; }
    }
}
