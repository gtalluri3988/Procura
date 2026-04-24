using DB.Entity;
using DB.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DB.Repositories.MasterDataRepository;

namespace DB.Repositories.Interfaces
{
    public interface IMasterDataRepository
    {
        Task<IEnumerable<CodeHierarchyDto>> GetFPMSBCategoryList();
        Task<IEnumerable<CodeHierarchyDto>> GetCodeHierarchyFlatAsync(int codeSystemId);

        // Tree operations
        Task<List<CodeHierarchyDto>> GetCodeTreeAsync(int codeSystemId);
        Task SaveCodeTreeAsync(IEnumerable<CodeHierarchyDto> nodes, int codeSystemId);

        // Single node operations
        Task<CodeHierarchyDto> AddNodeAsync(CodeHierarchyDto node, int codeSystemId, int? parentId = null);
        Task UpdateNodeAsync(int id, CodeHierarchyDto node);
        Task DeleteNodeAsync(int id);
        Task<IEnumerable<CodeHierarchyDto>> GetChildrenAsync(int parentId);


        Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync();

        Task SaveHierarchyAsync(int monthSetting, int YearSetting, List<CategoryDto> categories);

        Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync(int codeMasterId);

        Task SaveTenderManagementAsync(TenderManagementSaveRequest request);

        Task<TenderManagementResponse> GetTenderManagementAsync();




        Task AddMaterilBudgetAsync(MaterialBudgetDto model);
        Task SaveMaterialBudgetsAsync(IEnumerable<MaterialBudgetDto> models);
        Task UpdateMaterilBudgetAsync(MaterialBudgetDto model);
        Task<bool> DeleteMaterilBudgetAsync(int id);

        Task<MaterialBudgetDto> GetMaterilBudgetByIdAsync(int id);
        Task<IEnumerable<MaterialBudgetDto>> GetAllMaterilBudgetListAsync();
        Task<IEnumerable<MaterialBudgetDto>> GetByJobCategoryAsync(int jobCategoryId);

        Task UploadMaterialBudgetFilesAsync(IEnumerable<IFormFile> files, int uploadedBy);
        Task<IEnumerable<MaterialBudgetUploadDto>> GetMaterialBudgetUploadsAsync();

        Task SaveOrUpdateVendorManagementSettingAsync(VendorManagementSettingDto model);

        Task<VendorManagementSettingDto> GetVendorManagementSettingAsync();

        Task<(int monthsSetting, int yearsetting)> GetCategoryCodeSettingAsync();

        Task<IEnumerable<BankKeyDto>> GetAllBankKeysAsync();

    }
}
