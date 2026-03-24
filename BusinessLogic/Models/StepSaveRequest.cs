using DB.EFModel;
using DB.Entity;
using DB.Helper;

namespace Procura.Models
{
    public class StepSaveRequest
    {
        public VendorRegistrationStep Step { get; set; }

        public VendorProfileDto? Profile { get; set; }
        public VendorMemberDto? Members { get; set; }
        public VendorFinancialDto? Financial { get; set; }
        public List<VendorCategoryDto>? Categories { get; set; }
        public List<VendorExperienceDto>? Experiences { get; set; }

        public List<VendorCategoryCertificate>? VendorCategoryCertificate { get; set; }
        public VendorDeclarationDto? Declaration { get; set; }
    }
}
