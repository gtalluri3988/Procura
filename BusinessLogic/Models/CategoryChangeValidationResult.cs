using System;
using System.Collections.Generic;

namespace BusinessLogic.Models
{
    public class CategoryChangeValidationResult
    {
        public bool IsEligible { get; set; }
        public bool IsInFreezePeriod { get; set; }
        public bool HasExceededMaxChanges { get; set; }
        public int RemainingChanges { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public DateTime? ValidityEndDate { get; set; }
        public int CurrentCategoryCount { get; set; }
        public int MaxCategoriesAllowed { get; set; } = 2;
        public int MaxSubCategoriesPerCategory { get; set; } = 3;
        public List<string> Errors { get; set; } = new();
    }
}
