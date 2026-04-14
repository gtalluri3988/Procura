using System;

namespace DB.EFModel
{
    public class CategoryCodeApprovalItem : BaseEntity
    {
        public int Id { get; set; }
        public int CategoryCodeApprovalId { get; set; }
        public int CodeMasterId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? ActivityId { get; set; }
        public string? CertificatePath { get; set; }
        public DateTime? CertificateStartDate { get; set; }
        public DateTime? CertificateEndDate { get; set; }

        // Navigation properties
        public CategoryCodeApproval? CategoryCodeApproval { get; set; }
        public CodeMaster? CodeMaster { get; set; }
        public Category? Category { get; set; }
        public SubCategory? SubCategory { get; set; }
        public Activity? Activity { get; set; }
    }
}
