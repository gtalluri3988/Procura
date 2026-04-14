using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CategoryCodeApprovalService : ICategoryCodeApprovalService
    {
        private readonly ICategoryCodeApprovalRepository _approvalRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMasterDataRepository _masterDataRepository;

        private const int MaxCategoriesAllowed = 2;
        private const int MaxSubCategoriesPerCategory = 3;
        private const int MaxChangesAllowed = 2;

        public CategoryCodeApprovalService(
            ICategoryCodeApprovalRepository approvalRepository,
            IVendorRepository vendorRepository,
            IMasterDataRepository masterDataRepository)
        {
            _approvalRepository = approvalRepository;
            _vendorRepository = vendorRepository;
            _masterDataRepository = masterDataRepository;
        }

        public async Task<int> SubmitCategoryChangeRequestAsync(int vendorId, VendorCategoryRequest request)
        {
            // Step 1: Verify vendor exists
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                throw new Exception("Vendor not found.");

            // Step 2: If vendor is not yet SAP-approved, they should use the direct save flow
            if (!vendor.ApprovalDatetime.HasValue)
                throw new Exception("Vendor is still in initial registration. Use the standard category save endpoint.");

            // Step 3: Check no pending request already exists
            var hasPending = await _approvalRepository.HasPendingRequestAsync(vendorId);
            if (hasPending)
                throw new Exception("You already have a pending category change request. Please wait for it to be processed.");

            // Step 4: Run eligibility validation (freeze period, change limits)
            var eligibility = await ValidateAndCheckEligibilityAsync(vendorId);
            if (!eligibility.IsEligible)
                throw new Exception(string.Join(" ", eligibility.Errors));

            // Step 5: Validate category/subcategory limits against existing + requested
            var vendorFull = await _vendorRepository.GetVendorFullDetailsAsync(vendorId);
            ValidateCategoryLimits(vendorFull, request);

            // Step 6: Determine request type
            var existingCategoryCount = vendorFull?.VendorCategories?.Count ?? 0;
            var requestType = existingCategoryCount == 0 ? "AddCategory" : "UpdateCategory";

            // Step 7: Serialize the request as JSON snapshot for audit
            var requestedDataJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Step 8: Build approval items from the request
            var items = new List<CategoryCodeApprovalItemDto>();

            if (request.VendorCategoryDto != null)
            {
                // Group certificates by CodeMasterId for lookup
                var certLookup = request.VendorCategoryCertificateDto?
                    .GroupBy(c => c.CodeMasterId)
                    .ToDictionary(g => g.Key, g => g.First())
                    ?? new Dictionary<int, VendorCategoryCertificateDto>();

                foreach (var dto in request.VendorCategoryDto)
                {
                    var item = new CategoryCodeApprovalItemDto
                    {
                        CodeMasterId = dto.CodeMasterId,
                        CategoryId = dto.CategoryId,
                        SubCategoryId = dto.SubCategoryId,
                        ActivityId = dto.ActivityId
                    };

                    // Attach certificate info if available for this code master
                    if (certLookup.TryGetValue(dto.CodeMasterId, out var cert))
                    {
                        item.CertificatePath = cert.CertificatePath;
                        item.CertificateStartDate = cert.StartDate;
                        item.CertificateEndDate = cert.EndDate;
                    }

                    items.Add(item);
                }
            }

            return await _approvalRepository.CreateApprovalRequestAsync(
                vendorId, requestType, requestedDataJson, items);
        }

        public async Task<List<CategoryCodeApprovalListDto>> GetApprovalRequestsAsync(string? status)
        {
            return await _approvalRepository.GetApprovalRequestsAsync(status);
        }

        public async Task<CategoryCodeApprovalDto?> GetApprovalRequestByIdAsync(int requestId)
        {
            return await _approvalRepository.GetApprovalRequestByIdAsync(requestId);
        }

        public async Task ApproveCategoryChangeAsync(int requestId)
        {
            // Re-validate eligibility at approval time
            var approval = await _approvalRepository.GetApprovalRequestByIdAsync(requestId);
            if (approval == null)
                throw new Exception("Approval request not found.");

            var vendor = await _vendorRepository.GetVendorByIdAsync(approval.VendorId);
            if (vendor == null)
                throw new Exception("Vendor not found.");

            if (!vendor.Status)
                throw new Exception("Vendor is no longer active. Cannot approve category change.");

            // Re-validate change limits at approval time (another request may have been approved since)
            var eligibility = await ValidateAndCheckEligibilityAsync(approval.VendorId);
            if (!eligibility.IsEligible)
                throw new Exception("Vendor is no longer eligible for category changes: " + string.Join(" ", eligibility.Errors));

            await _approvalRepository.ApproveRequestAsync(requestId);
        }

        public async Task RejectCategoryChangeAsync(int requestId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new Exception("Rejection reason is required.");

            await _approvalRepository.RejectRequestAsync(requestId, reason);
        }

        public async Task<CategoryChangeValidationResult> ValidateAndCheckEligibilityAsync(int vendorId)
        {
            var result = new CategoryChangeValidationResult
            {
                MaxCategoriesAllowed = MaxCategoriesAllowed,
                MaxSubCategoriesPerCategory = MaxSubCategoriesPerCategory
            };

            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null)
            {
                result.IsEligible = false;
                result.Errors.Add("Vendor not found.");
                return result;
            }

            var (monthsSetting, yearsSetting) = await _masterDataRepository.GetCategoryCodeSettingAsync();

            // Freeze period check
            if (vendor.ApprovalDatetime.HasValue && monthsSetting > 0)
            {
                var freezeEnd = vendor.ApprovalDatetime.Value.AddMonths(monthsSetting);
                result.FreezeEndDate = freezeEnd;

                if (DateTime.UtcNow < freezeEnd)
                {
                    result.IsInFreezePeriod = true;
                    result.IsEligible = false;
                    result.Errors.Add(
                        $"Category codes are frozen until {freezeEnd:dd MMM yyyy}. " +
                        $"Changes are not allowed during the {monthsSetting}-month freeze period after approval.");
                    return result;
                }
            }

            // Change limit check (count approved changes + pending requests)
            if (vendor.ApprovalDatetime.HasValue && yearsSetting > 0)
            {
                var validityStart = vendor.ApprovalDatetime.Value;
                var validityEnd = validityStart.AddYears(yearsSetting);
                result.ValidityEndDate = validityEnd;

                var approvedChangeCount = await _vendorRepository.GetCategoryChangeCountAsync(
                    vendorId, validityStart, validityEnd);

                var pendingCount = await _approvalRepository.GetPendingRequestCountAsync(vendorId);
                var totalChanges = approvedChangeCount + pendingCount;

                result.RemainingChanges = Math.Max(0, MaxChangesAllowed - totalChanges);

                if (totalChanges >= MaxChangesAllowed)
                {
                    result.HasExceededMaxChanges = true;
                    result.IsEligible = false;
                    result.Errors.Add(
                        $"Maximum {MaxChangesAllowed} category code changes allowed within the " +
                        $"{yearsSetting}-year validity period. " +
                        $"Next change available after {validityEnd:dd MMM yyyy}.");
                    return result;
                }
            }

            // Current category count
            var vendorFull = await _vendorRepository.GetVendorFullDetailsAsync(vendorId);
            if (vendorFull?.VendorCategories != null)
            {
                result.CurrentCategoryCount = vendorFull.VendorCategories
                    .Select(c => c.CategoryId)
                    .Distinct()
                    .Count();
            }

            result.IsEligible = true;
            if (!vendor.ApprovalDatetime.HasValue)
            {
                result.RemainingChanges = MaxChangesAllowed;
            }

            return result;
        }

        private void ValidateCategoryLimits(Vendor? vendorFull, VendorCategoryRequest request)
        {
            if (request.VendorCategoryDto == null || request.VendorCategoryDto.Count == 0)
                return;

            // Count distinct categories being requested
            var requestedCategories = request.VendorCategoryDto
                .Where(c => c.CategoryId.HasValue)
                .Select(c => c.CategoryId!.Value)
                .Distinct()
                .ToList();

            // Count existing categories
            var existingCategories = vendorFull?.VendorCategories?
                .Where(c => c.CategoryId.HasValue)
                .Select(c => c.CategoryId!.Value)
                .Distinct()
                .ToList() ?? new List<int>();

            // New categories = requested that don't exist yet
            var newCategories = requestedCategories.Except(existingCategories).ToList();
            var totalCategories = existingCategories.Count + newCategories.Count;

            if (totalCategories > MaxCategoriesAllowed)
                throw new Exception($"Maximum {MaxCategoriesAllowed} categories allowed per vendor. Current: {existingCategories.Count}, New: {newCategories.Count}.");

            // Validate subcategory limits per category
            var requestedGrouped = request.VendorCategoryDto
                .Where(c => c.CategoryId.HasValue)
                .GroupBy(c => c.CategoryId!.Value);

            foreach (var group in requestedGrouped)
            {
                var existingSubsForCategory = vendorFull?.VendorCategories?
                    .Where(c => c.CategoryId == group.Key && c.SubCategoryId.HasValue)
                    .Select(c => c.SubCategoryId!.Value)
                    .Distinct()
                    .ToList() ?? new List<int>();

                var requestedSubs = group
                    .Where(c => c.SubCategoryId.HasValue)
                    .Select(c => c.SubCategoryId!.Value)
                    .Distinct()
                    .ToList();

                var newSubs = requestedSubs.Except(existingSubsForCategory).ToList();
                var totalSubs = existingSubsForCategory.Count + newSubs.Count;

                if (totalSubs > MaxSubCategoriesPerCategory)
                    throw new Exception($"Maximum {MaxSubCategoriesPerCategory} subcategories allowed per category. Category {group.Key} would have {totalSubs}.");
            }
        }
    }
}
