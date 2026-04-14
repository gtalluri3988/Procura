using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class CategoryCodeApprovalRepository : RepositoryBase<CategoryCodeApproval, CategoryCodeApprovalDto>, ICategoryCodeApprovalRepository
    {
        private readonly IConfiguration _configuration;

        public CategoryCodeApprovalRepository(
            ProcuraDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(context, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateApprovalRequestAsync(
            int vendorId,
            string requestType,
            string requestedDataJson,
            List<CategoryCodeApprovalItemDto> items)
        {
            var userId = GetCurrentUserId();

            var approval = new CategoryCodeApproval
            {
                VendorId = vendorId,
                RequestType = requestType,
                Status = "Pending",
                RequestedData = requestedDataJson,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            await _context.CategoryCodeApprovals.AddAsync(approval);
            await _context.SaveChangesAsync();

            // Save certificate files and create line items
            var uploadRoot = _configuration["FileSettings:UploadPath"];
            if (string.IsNullOrWhiteSpace(uploadRoot))
                uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            var certFolder = Path.Combine(uploadRoot, "PendingCertificates");
            if (!Directory.Exists(certFolder))
                Directory.CreateDirectory(certFolder);

            foreach (var item in items)
            {
                var entity = new CategoryCodeApprovalItem
                {
                    CategoryCodeApprovalId = approval.Id,
                    CodeMasterId = item.CodeMasterId,
                    CategoryId = item.CategoryId,
                    SubCategoryId = item.SubCategoryId,
                    ActivityId = item.ActivityId,
                    CertificateStartDate = item.CertificateStartDate,
                    CertificateEndDate = item.CertificateEndDate,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                // Handle base64 certificate file
                if (!string.IsNullOrEmpty(item.CertificatePath) &&
                    (item.CertificatePath.Contains(",") || item.CertificatePath.Length > 200))
                {
                    var safeFileName = $"pending_{approval.Id}_codemaster_{item.CodeMasterId}_{Guid.NewGuid():N}.pdf";
                    var fullPath = Path.Combine(certFolder, safeFileName);
                    SaveBase64ToFile(item.CertificatePath, fullPath);
                    entity.CertificatePath = Path.GetRelativePath(uploadRoot, fullPath).Replace("\\", "/");
                }
                else
                {
                    entity.CertificatePath = item.CertificatePath;
                }

                await _context.CategoryCodeApprovalItems.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
            return approval.Id;
        }

        public async Task<List<CategoryCodeApprovalListDto>> GetApprovalRequestsAsync(string? status)
        {
            var query = _context.CategoryCodeApprovals
                .Include(a => a.Vendor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => new CategoryCodeApprovalListDto
                {
                    Id = a.Id,
                    VendorId = a.VendorId,
                    VendorName = a.Vendor != null ? a.Vendor.CompanyName : null,
                    RegistrationNo = a.Vendor != null ? a.Vendor.RocNumber : null,
                    RequestType = a.RequestType,
                    Status = a.Status,
                    RequestDateTime = a.CreatedDate,
                    ApprovedDateTime = a.ReviewedDate
                })
                .ToListAsync();
        }

        public async Task<CategoryCodeApprovalDto?> GetApprovalRequestByIdAsync(int requestId)
        {
            var approval = await _context.CategoryCodeApprovals
                .Include(a => a.Vendor)
                .Include(a => a.Items)
                    .ThenInclude(i => i.CodeMaster)
                .Include(a => a.Items)
                    .ThenInclude(i => i.Category)
                .Include(a => a.Items)
                    .ThenInclude(i => i.SubCategory)
                .Include(a => a.Items)
                    .ThenInclude(i => i.Activity)
                .FirstOrDefaultAsync(a => a.Id == requestId);

            if (approval == null)
                return null;

            return new CategoryCodeApprovalDto
            {
                Id = approval.Id,
                VendorId = approval.VendorId,
                RequestType = approval.RequestType,
                Status = approval.Status,
                RequestedData = approval.RequestedData,
                RejectionReason = approval.RejectionReason,
                ReviewedBy = approval.ReviewedBy,
                ReviewedDate = approval.ReviewedDate,
                CreatedBy = approval.CreatedBy,
                CreatedDate = approval.CreatedDate,
                VendorName = approval.Vendor?.CompanyName,
                RegistrationNo = approval.Vendor?.RocNumber,
                Items = approval.Items.Select(i => new CategoryCodeApprovalItemDto
                {
                    Id = i.Id,
                    CategoryCodeApprovalId = i.CategoryCodeApprovalId,
                    CodeMasterId = i.CodeMasterId,
                    CodeMasterName = i.CodeMaster?.Name,
                    CategoryId = i.CategoryId,
                    CategoryName = i.Category?.CategoryName,
                    SubCategoryId = i.SubCategoryId,
                    SubCategoryName = i.SubCategory?.SubCategoryName,
                    ActivityId = i.ActivityId,
                    ActivityName = i.Activity?.ActivityName,
                    CertificatePath = i.CertificatePath,
                    CertificateStartDate = i.CertificateStartDate,
                    CertificateEndDate = i.CertificateEndDate
                }).ToList()
            };
        }

        public async Task<bool> HasPendingRequestAsync(int vendorId)
        {
            return await _context.CategoryCodeApprovals
                .AnyAsync(a => a.VendorId == vendorId && a.Status == "Pending");
        }

        public async Task<int> GetPendingRequestCountAsync(int vendorId)
        {
            return await _context.CategoryCodeApprovals
                .CountAsync(a => a.VendorId == vendorId && a.Status == "Pending");
        }

        public async Task ApproveRequestAsync(int requestId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var approval = await _context.CategoryCodeApprovals
                    .Include(a => a.Items)
                    .FirstOrDefaultAsync(a => a.Id == requestId);

                if (approval == null)
                    throw new Exception("Approval request not found.");

                if (approval.Status != "Pending")
                    throw new Exception($"Request has already been processed. Current status: {approval.Status}");

                var userId = GetCurrentUserId();

                // Insert approved categories into VendorCategory
                foreach (var item in approval.Items)
                {
                    var exists = await _context.VendorCategories.AnyAsync(vc =>
                        vc.VendorId == approval.VendorId &&
                        vc.CodeMasterId == item.CodeMasterId &&
                        vc.CategoryId == item.CategoryId &&
                        vc.SubCategoryId == item.SubCategoryId);

                    if (!exists)
                    {
                        await _context.VendorCategories.AddAsync(new VendorCategory
                        {
                            VendorId = approval.VendorId,
                            CodeMasterId = item.CodeMasterId,
                            CategoryId = item.CategoryId,
                            SubCategoryId = item.SubCategoryId,
                            ActivityId = item.ActivityId
                        });
                    }
                }

                // Insert/update certificates
                var certItems = approval.Items
                    .Where(i => !string.IsNullOrEmpty(i.CertificatePath))
                    .ToList();

                foreach (var certItem in certItems)
                {
                    var existingCert = await _context.VendorCategoryCertificates
                        .FirstOrDefaultAsync(c =>
                            c.VendorId == approval.VendorId &&
                            c.CodeMasterId == certItem.CodeMasterId);

                    if (existingCert != null)
                    {
                        existingCert.CertificatePath = certItem.CertificatePath;
                        existingCert.StartDate = certItem.CertificateStartDate ?? existingCert.StartDate;
                        existingCert.EndDate = certItem.CertificateEndDate ?? existingCert.EndDate;
                        existingCert.UpdatedBy = userId;
                        existingCert.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        await _context.VendorCategoryCertificates.AddAsync(new VendorCategoryCertificate
                        {
                            VendorId = approval.VendorId,
                            CodeMasterId = certItem.CodeMasterId,
                            CertificatePath = certItem.CertificatePath,
                            StartDate = certItem.CertificateStartDate ?? DateTime.UtcNow,
                            EndDate = certItem.CertificateEndDate ?? DateTime.UtcNow.AddYears(1),
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }

                // Update approval status
                approval.Status = "Approved";
                approval.ReviewedBy = int.TryParse(userId, out var uid) ? uid : null;
                approval.ReviewedDate = DateTime.UtcNow;
                approval.UpdatedBy = userId;
                approval.UpdatedDate = DateTime.UtcNow;

                // Log the change for freeze/limit tracking (only on approval)
                var changeLog = new VendorCategoryChangeLog
                {
                    VendorId = approval.VendorId,
                    ChangeDate = DateTime.UtcNow,
                    ChangeDescription = $"Category change approved (Request #{requestId}). Items: {approval.Items.Count}",
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };
                await _context.VendorCategoryChangeLogs.AddAsync(changeLog);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task RejectRequestAsync(int requestId, string reason)
        {
            var approval = await _context.CategoryCodeApprovals
                .FirstOrDefaultAsync(a => a.Id == requestId);

            if (approval == null)
                throw new Exception("Approval request not found.");

            if (approval.Status != "Pending")
                throw new Exception($"Request has already been processed. Current status: {approval.Status}");

            var userId = GetCurrentUserId();

            approval.Status = "Rejected";
            approval.RejectionReason = reason;
            approval.ReviewedBy = int.TryParse(userId, out var uid) ? uid : null;
            approval.ReviewedDate = DateTime.UtcNow;
            approval.UpdatedBy = userId;
            approval.UpdatedDate = DateTime.UtcNow;

            // Do NOT log to VendorCategoryChangeLog — rejected requests don't count toward limits

            await _context.SaveChangesAsync();
        }

        private static void SaveBase64ToFile(string base64String, string filePath)
        {
            if (base64String.Contains(","))
                base64String = base64String.Substring(base64String.IndexOf(",") + 1);

            byte[] fileBytes = Convert.FromBase64String(base64String);
            File.WriteAllBytes(filePath, fileBytes);
        }
    }
}
