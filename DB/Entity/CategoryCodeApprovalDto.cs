using System;
using System.Collections.Generic;

namespace DB.Entity
{
    /// <summary>
    /// Full detail DTO for a single approval request (used on the approval detail page).
    /// </summary>
    public class CategoryCodeApprovalDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? RequestedData { get; set; }
        public string? RejectionReason { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Vendor info (populated for detail view)
        public string? VendorName { get; set; }
        public string? RegistrationNo { get; set; }

        public List<CategoryCodeApprovalItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Line-item DTO for an individual category within an approval request.
    /// </summary>
    public class CategoryCodeApprovalItemDto
    {
        public int Id { get; set; }
        public int CategoryCodeApprovalId { get; set; }
        public int CodeMasterId { get; set; }
        public string? CodeMasterName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public int? ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public string? CertificatePath { get; set; }
        public DateTime? CertificateStartDate { get; set; }
        public DateTime? CertificateEndDate { get; set; }
    }

    /// <summary>
    /// Lightweight DTO for the approval list grid (ReviewList.png).
    /// </summary>
    public class CategoryCodeApprovalListDto
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? RegistrationNo { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDateTime { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
    }

    /// <summary>
    /// Request body for approving a category change request.
    /// </summary>
    public class ApproveCategoryChangeRequest
    {
        public int RequestId { get; set; }
    }

    /// <summary>
    /// Request body for rejecting a category change request.
    /// </summary>
    public class RejectCategoryChangeRequest
    {
        public int RequestId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
