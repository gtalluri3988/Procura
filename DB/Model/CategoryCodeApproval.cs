using System;
using System.Collections.Generic;

namespace DB.EFModel
{
    public class CategoryCodeApproval : BaseEntity
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string RequestType { get; set; } = string.Empty; // "AddCategory" | "UpdateCategory"
        public string Status { get; set; } = "Pending"; // "Pending" | "Approved" | "Rejected"
        public string? RequestedData { get; set; } // JSON snapshot of VendorCategoryRequest
        public string? RejectionReason { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }

        // Navigation properties
        public Vendor? Vendor { get; set; }
        public User? Reviewer { get; set; }
        public ICollection<CategoryCodeApprovalItem> Items { get; set; } = new List<CategoryCodeApprovalItem>();
    }
}
