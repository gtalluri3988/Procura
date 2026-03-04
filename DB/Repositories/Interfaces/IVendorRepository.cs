using DB.EFModel;
using DB.Entity;
using DB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<int> CreateVendorAsync(Vendor vendor);
        Task<VendorRegistrationStep?> UpdateVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(int vendorId);

        Task<Vendor?> GetVendorByIdAsync(int vendorId);
        Task<Vendor?> GetVendorFullDetailsAsync(int vendorId);

        Task<VendorRegistrationStep?> SaveMembersAsync(
    int vendorId,
    List<VendorShareholder> shareholders,
    List<VendorDirector> directors,
    VendorManpower manpower,
    List<VendorStaffDeclaration> staffDeclarations);
        Task<VendorRegistrationStep?> SaveFinancialAsync(int vendorId, VendorFinancial financial);
        Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, List<VendorCategory> categories);
        Task<VendorRegistrationStep?> SaveExperiencesAsync(int vendorId, List<VendorExperience> experiences);
        Task<VendorRegistrationStep?> SaveDeclarationAsync(int vendorId, VendorDeclaration declaration);
        // Task SavePaymentAsync(int vendorId, VendorPayment payment);
        Task SaveSSMResponse(string input, string response);

        Task<VendorProfileDto> RegisterVendor(Vendor vendor);

        Task<PaymentDetailsDTO> GetPaymentDetailsAsync(int vendorId);

        Task<IEnumerable<CompanyCategoryDto>> GetCompanyTypes();

        Task<IEnumerable<CompanyCategoryDto>> GetCompanyEntitiesByTypeIdAsync(int TypeId);

        Task<IEnumerable<VendorProfileDto>> GetVendorListAsync();

        Task<VendorProfileDto> GetVendorByVendorIdAsync(int vendorId);

        VendorProfileDto GetVendorByROCandPasswordAsync(string roc, string password);

        Task<VendorDashboardDto> GetVendorDashboardAsync();

        Task<IEnumerable<VendorProfileDto>> GetVendorListAsync(VendorSearchRequest? request);

        Task<Vendor?> GetSAPVendorByVendorIdAsync(int vendorId);
        Task SaveSAPRequestResponseAsync(int VendorId, string request, string response);
    }
}
