using System;
using System.Collections.Generic;
using System.Linq;
using DB.EFModel;
using DB.Entity;

namespace BusinessLogic.Validators
{
    internal static class VendorCategoryLimitValidator
    {
        public const int MaxCategoriesPerCodeMaster = 2;
        public const int MaxSubCategoriesPerCategory = 3;

        public static void EnsureLimits(VendorCategoryRequest? request, Vendor? vendorFull)
        {
            if (request?.VendorCategoryDto == null || request.VendorCategoryDto.Count == 0)
                return;

            EnsureCategoriesPerCodeMaster(request, vendorFull);
            EnsureSubCategoriesPerCategory(request, vendorFull);
        }

        private static void EnsureCategoriesPerCodeMaster(VendorCategoryRequest request, Vendor? vendorFull)
        {
            var requestedByCm = request.VendorCategoryDto!
                .Where(c => c.CategoryId.HasValue)
                .GroupBy(c => c.CodeMasterId);

            foreach (var group in requestedByCm)
            {
                var existing = vendorFull?.VendorCategories?
                    .Where(c => c.CodeMasterId == group.Key && c.CategoryId.HasValue)
                    .Select(c => c.CategoryId!.Value)
                    .Distinct()
                    .ToList() ?? new List<int>();

                var requested = group
                    .Select(c => c.CategoryId!.Value)
                    .Distinct()
                    .ToList();

                var total = existing.Union(requested).Count();

                if (total > MaxCategoriesPerCodeMaster)
                {
                    var newCount = requested.Except(existing).Count();
                    throw new Exception(
                        $"Maximum {MaxCategoriesPerCodeMaster} categories allowed per code master. " +
                        $"CodeMaster {group.Key}: existing {existing.Count}, new {newCount}, " +
                        $"would total {total}.");
                }
            }
        }

        private static void EnsureSubCategoriesPerCategory(VendorCategoryRequest request, Vendor? vendorFull)
        {
            var requestedGrouped = request.VendorCategoryDto!
                .Where(c => c.CategoryId.HasValue)
                .GroupBy(c => c.CategoryId!.Value);

            foreach (var group in requestedGrouped)
            {
                var existingSubs = vendorFull?.VendorCategories?
                    .Where(c => c.CategoryId == group.Key && c.SubCategoryId.HasValue)
                    .Select(c => c.SubCategoryId!.Value)
                    .Distinct()
                    .ToList() ?? new List<int>();

                var requestedSubs = group
                    .Where(c => c.SubCategoryId.HasValue)
                    .Select(c => c.SubCategoryId!.Value)
                    .Distinct()
                    .ToList();

                var totalSubs = existingSubs.Union(requestedSubs).Count();

                if (totalSubs > MaxSubCategoriesPerCategory)
                {
                    throw new Exception(
                        $"Maximum {MaxSubCategoriesPerCategory} subcategories allowed per category. " +
                        $"Category {group.Key} would have {totalSubs}.");
                }
            }
        }
    }
}
