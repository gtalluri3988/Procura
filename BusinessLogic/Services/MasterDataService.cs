using Azure.Core;
using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using static DB.Repositories.MasterDataRepository;


namespace BusinessLogic.Services
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IMasterDataRepository _masterDataRepository;
        public MasterDataService(IMasterDataRepository masterDataRepository)
        {
            _masterDataRepository = masterDataRepository;
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetCodeHierarchyFlatAsync(int codeSystemId)
        {
            return await _masterDataRepository.GetCodeHierarchyFlatAsync(codeSystemId);
        }

        public async Task<List<CodeHierarchyDto>> GetCodeTreeAsync(int codeSystemId)
        {
            return await _masterDataRepository.GetCodeTreeAsync(codeSystemId);
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetFPMSBCategoryList()
        {
            return await _masterDataRepository.GetFPMSBCategoryList();
        }

        public async Task SaveCodeTreeAsync(IEnumerable<CodeHierarchyDto> nodes, int codeSystemId)
        {
            await _masterDataRepository.SaveCodeTreeAsync(nodes, codeSystemId);
        }

        // New single-node service methods
        public async Task<CodeHierarchyDto> AddNodeAsync(CodeHierarchyDto node, int codeSystemId, int? parentId = null)
        {
            return await _masterDataRepository.AddNodeAsync(node, codeSystemId, parentId);
        }

        public async Task UpdateNodeAsync(int id, CodeHierarchyDto node)
        {
            await _masterDataRepository.UpdateNodeAsync(id, node);
        }

        public async Task DeleteNodeAsync(int id)
        {
            await _masterDataRepository.DeleteNodeAsync(id);
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetChildrenAsync(int parentId)
        {
            return await _masterDataRepository.GetChildrenAsync(parentId);
        }



        public async Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync()
        {
            return await _masterDataRepository.GetAllHierarchyAsync();
        }

        public async Task SaveHierarchyAsync(int monthSetting, int YearSetting, List<CategoryDto> categories)
        {
            await _masterDataRepository.SaveHierarchyAsync(monthSetting, YearSetting, categories);
        }

        public async Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync(int codeMasterId)
        {
            return await _masterDataRepository.GetAllHierarchyAsync(codeMasterId);

        }

        public async Task SaveTenderManagementAsync(TenderManagementSaveRequest request)
        {
            await _masterDataRepository.SaveTenderManagementAsync(request);
        }

        public async Task<TenderManagementResponse> GetTenderManagementAsync()
        {
            return await _masterDataRepository.GetTenderManagementAsync();
        }

        public async Task<MaterialBudgetDto> GetMaterilBudgetByIdAsync(int id)
        {
            return await _masterDataRepository.GetMaterilBudgetByIdAsync(id);
        }

        public async Task<IEnumerable<MaterialBudgetDto>> GetAllMaterilBudgetListAsync()
        {
            return await _masterDataRepository.GetAllMaterilBudgetListAsync();
        }

        public async Task<IEnumerable<MaterialBudgetDto>> GetByJobCategoryAsync(int jobCategoryId)
        {
            return await _masterDataRepository.GetByJobCategoryAsync(jobCategoryId);
        }

        public async Task AddMaterilBudgetAsync(MaterialBudgetDto model)
        {
            await _masterDataRepository.AddMaterilBudgetAsync(model);
        }

        public async Task UpdateMaterilBudgetAsync(MaterialBudgetDto model)
        {
            await _masterDataRepository.UpdateMaterilBudgetAsync(model);
        }

        public async Task<bool> DeleteMaterilBudgetAsync(int id)
        {
            return await _masterDataRepository.DeleteMaterilBudgetAsync(id);
        }

        public async Task UploadMaterialBudgetFilesAsync(IEnumerable<IFormFile> files, int uploadedBy)
        {
            await _masterDataRepository.UploadMaterialBudgetFilesAsync(files, uploadedBy);
        }

        public async Task<IEnumerable<MaterialBudgetUploadDto>> GetMaterialBudgetUploadsAsync()
        {
            return await _masterDataRepository.GetMaterialBudgetUploadsAsync();
        }

        public async Task SaveOrUpdateVendorManagementSettingAsync(VendorManagementSettingDto model)
        {
            await _masterDataRepository.SaveOrUpdateVendorManagementSettingAsync(model);
        }

        public async Task<VendorManagementSettingDto> GetVendorManagementSettingAsync()
        {
            return await _masterDataRepository.GetVendorManagementSettingAsync();
        }

        public async Task<(int monthsSetting, int yearsetting)> GetCategoryCodeSettingAsync()
        {
            return await _masterDataRepository.GetCategoryCodeSettingAsync();
        }

        public async Task<IEnumerable<BankKeyDto>> GetAllBankKeysAsync()
        {
            return await _masterDataRepository.GetAllBankKeysAsync();
        }
    }
}