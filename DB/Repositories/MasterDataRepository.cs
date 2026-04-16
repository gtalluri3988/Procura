using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Numerics;


namespace DB.Repositories
{
    public class MasterDataRepository : RepositoryBase<CodeHierarchy, CodeHierarchyDto>, IMasterDataRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public MasterDataRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(context, mapper, httpContextAccessor)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetFPMSBCategoryList()
        {
            var companyTypes = await _context.CodeHierarchies.Where(x => x.CodeSystemId == 1 && x.ParentId == null).ToListAsync();
            return _mapper.Map<IEnumerable<CodeHierarchyDto>>(companyTypes);
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetCodeHierarchyFlatAsync(int codeSystemId)
        {
            var items = await _context.CodeHierarchies
                .AsNoTracking()
                .Where(c => c.CodeSystemId == codeSystemId)
                .OrderBy(c => c.Level)
                .ThenBy(c => c.Code)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CodeHierarchyDto>>(items);
        }

        public async Task<List<CodeHierarchyDto>> GetCodeTreeAsync(int codeSystemId)
        {
            var all = await _context.CodeHierarchies
                .AsNoTracking()
                .Where(c => c.CodeSystemId == codeSystemId)
                .ToListAsync();

            var lookup = all.ToDictionary(
                c => c.Id,
                c => new CodeHierarchyDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Description = c.Description,
                    Level = c.Level,
                    IsActive = c.IsActive,
                    Children = new List<CodeHierarchyDto>()
                });

            var roots = new List<CodeHierarchyDto>();
            foreach (var item in all)
            {
                var node = lookup[item.Id];
                if (item.ParentId.HasValue && lookup.TryGetValue(item.ParentId.Value, out var parent))
                    parent.Children.Add(node);
                else
                    roots.Add(node);
            }

            void SortRecursively(List<CodeHierarchyDto> nodes)
            {
                nodes.Sort((a, b) => string.Compare(a.Code, b.Code, StringComparison.Ordinal));
                foreach (var n in nodes) SortRecursively(n.Children);
            }
            SortRecursively(roots);

            return roots;
        }

        public async Task SaveCodeTreeAsync(IEnumerable<CodeHierarchyDto> nodes, int codeSystemId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var node in nodes)
                {
                    await UpsertNodeRecursiveAsync(node, codeSystemId, parentId: null);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<CodeHierarchy> UpsertNodeRecursiveAsync(CodeHierarchyDto dto, int codeSystemId, int? parentId)
        {
            var existing = await _context.CodeHierarchies
                .FirstOrDefaultAsync(c =>
                    c.CodeSystemId == codeSystemId
                    && c.Code == dto.Code
                    && ((c.ParentId == null && parentId == null) || (c.ParentId == parentId))
                );

            if (existing == null)
            {
                existing = new CodeHierarchy
                {
                    CodeSystemId = codeSystemId,
                    ParentId = parentId,
                    Code = dto.Code,
                    Description = dto.Description,
                    Level = dto.Level,
                    IsActive = dto.IsActive ?? true
                };
                await _context.CodeHierarchies.AddAsync(existing);
                await _context.SaveChangesAsync();
            }
            else
            {
                existing.Description = dto.Description;
                existing.Level = dto.Level;
                existing.IsActive = dto.IsActive ?? existing.IsActive;
                _context.CodeHierarchies.Update(existing);
                await _context.SaveChangesAsync();
            }

            foreach (var child in dto.Children)
            {
                await UpsertNodeRecursiveAsync(child, codeSystemId, existing.Id);
            }

            return existing;
        }

        // --- New single-node operations ---

        public async Task<CodeHierarchyDto> AddNodeAsync(CodeHierarchyDto node, int codeSystemId, int? parentId = null)
        {
            // prevent duplicate code under same parent
            var exists = await _context.CodeHierarchies.AnyAsync(c =>
                c.CodeSystemId == codeSystemId &&
                c.Code == node.Code &&
                ((c.ParentId == null && parentId == null) || (c.ParentId == parentId))
            );

            if (exists)
                throw new InvalidOperationException("Duplicate code under same parent.");

            // compute level
            int level = 1;
            if (parentId.HasValue)
            {
                var parent = await _context.CodeHierarchies.FindAsync(parentId.Value);
                if (parent == null) throw new InvalidOperationException("Parent not found.");
                level = parent.Level + 1;
            }
            else if (node.Level > 0) level = node.Level;

            var entity = new CodeHierarchy
            {
                CodeSystemId = codeSystemId,
                ParentId = parentId,
                Code = node.Code,
                Description = node.Description,
                Level = level,
                IsActive = node.IsActive ?? true
            };

            _context.CodeHierarchies.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<CodeHierarchyDto>(entity);
        }

        public async Task UpdateNodeAsync(int id, CodeHierarchyDto node)
        {
            var existing = await _context.CodeHierarchies.FindAsync(id);
            if (existing == null) throw new InvalidOperationException("Node not found.");

            // update fields; do not change ParentId here (use separate endpoint if needed)
            existing.Code = node.Code;
            existing.Description = node.Description;
            existing.Level = node.Level;
            existing.IsActive = node.IsActive ?? existing.IsActive;

            _context.CodeHierarchies.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNodeAsync(int id)
        {
            var existing = await _context.CodeHierarchies.FindAsync(id);
            if (existing == null) throw new InvalidOperationException("Node not found.");

            var hasChildren = await _context.CodeHierarchies.AnyAsync(c => c.ParentId == id);
            if (hasChildren)
                throw new InvalidOperationException("Cannot delete node with children. Delete children first.");

            _context.CodeHierarchies.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CodeHierarchyDto>> GetChildrenAsync(int parentId)
        {
            var children = await _context.CodeHierarchies
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CodeHierarchyDto>>(children);
        }

        public async Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync()
        {
            return await _context.CodeMasters
                .Select(code => new CodeMasterHierarchyDto
                {
                    CodeId = code.Id,
                    Code = code.Code,

                    Categories = code.Categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        CodeMasterId = c.CodeMasterId,
                       
                        CategoryName = c.CategoryName,

                        SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                        {
                            Id = sc.Id,
                            CategoryId = sc.CategoryId,

                           
                            SubCategoryName = sc.SubCategoryName,

                            Activities = sc.Activities.Select(a => new ActivityDto
                            {
                                Id = a.Id,
                                SubCategoryId = a.SubCategoryId,

                              
                                ActivityName = a.ActivityName
                            }).ToList()

                        }).ToList()

                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<List<CodeMasterHierarchyDto>> GetAllHierarchyAsync(int codeMasterId)
        {
            return await _context.CodeMasters.Where(x => x.Id == codeMasterId)
                .Select(code => new CodeMasterHierarchyDto
                {
                    CodeId = code.Id,
                    Code = code.Code,

                    Categories = code.Categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        CodeMasterId = c.CodeMasterId,
                       
                        CategoryName = c.CategoryName,

                        SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                        {
                            Id = sc.Id,
                            CategoryId = sc.CategoryId,

                           
                            SubCategoryName = sc.SubCategoryName,

                            Activities = sc.Activities.Select(a => new ActivityDto
                            {
                                Id = a.Id,
                                SubCategoryId = a.SubCategoryId,

                               
                                ActivityName = a.ActivityName
                            }).ToList()

                        }).ToList()

                    }).ToList()
                })
                .ToListAsync();
        }

        //public async Task SaveHierarchyAsync(int monthSetting, int YearSetting, List<CategoryDto> categories)
        //{
        //    try
        //    {
        //        var categorycodesetting = await _context.CategoryCodeSetting.FirstOrDefaultAsync();
        //        if (categorycodesetting == null)
        //        {
        //            var codesetting = new CategoryCodeSetting()
        //            {

        //                EditCategoryCodeAfterMonth = monthSetting,
        //                EditCategoryCodeLimitAfterYear = YearSetting
        //            };
        //            _context.CategoryCodeSetting.Add(codesetting);

        //        }
        //        else
        //        {
        //            categorycodesetting.EditCategoryCodeAfterMonth = monthSetting;
        //            categorycodesetting.EditCategoryCodeLimitAfterYear = YearSetting;

        //        }
        //        await _context.SaveChangesAsync();
        //    }
        //    catch(Exception ex)
        //    {

        //    }



        //    foreach (var cat in categories)
        //        {
        //            var category = new Category
        //            {
        //                CodeMasterId = cat.CodeMasterId,

        //                CategoryName = cat.CategoryName,

        //            };

        //            _context.Categories.Add(category);
        //            await _context.SaveChangesAsync();

        //            if (cat.SubCategories != null)
        //            {
        //                foreach (var sub in cat.SubCategories)
        //                {
        //                    var subCategory = new SubCategory
        //                    {
        //                        CategoryId = category.Id,


        //                        SubCategoryName = sub.SubCategoryName
        //                    };

        //                    _context.SubCategories.Add(subCategory);
        //                    await _context.SaveChangesAsync();

        //                    if (sub.Activities != null)
        //                    {
        //                        foreach (var act in sub.Activities)
        //                        {
        //                            var activity = new Activity
        //                            {
        //                                SubCategoryId = subCategory.Id,


        //                                ActivityName = act.ActivityName
        //                            };

        //                            _context.Activities.Add(activity);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    await _context.SaveChangesAsync();
        //}


        public async Task SaveHierarchyAsync(int monthSetting, int YearSetting, List<CategoryDto> categories)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var categorycodesetting = await _context.CategoryCodeSetting.FirstOrDefaultAsync();

                if (categorycodesetting == null)
                {
                    var codesetting = new CategoryCodeSetting()
                    {
                        EditCategoryCodeAfterMonth = monthSetting,
                        EditCategoryCodeLimitAfterYear = YearSetting
                    };

                    _context.CategoryCodeSetting.Add(codesetting);
                }
                else
                {
                    categorycodesetting.EditCategoryCodeAfterMonth = monthSetting;
                    categorycodesetting.EditCategoryCodeLimitAfterYear = YearSetting;
                }

                await _context.SaveChangesAsync();

                // Load existing hierarchy
                var existingActivities = await _context.Activities.ToListAsync();
                var existingSubCategories = await _context.SubCategories.ToListAsync();
                var existingCategories = await _context.Categories.ToListAsync();

                // Collect incoming IDs (id > 0 = existing record to keep/update; 0 or null = new)
                var incomingActivityIds = categories
                    .SelectMany(c => c.SubCategories ?? new List<SubCategoryDto>())
                    .SelectMany(s => s.Activities ?? new List<ActivityDto>())
                    .Where(a => a.Id.GetValueOrDefault() > 0)
                    .Select(a => a.Id!.Value)
                    .ToHashSet();

                var incomingSubCategoryIds = categories
                    .SelectMany(c => c.SubCategories ?? new List<SubCategoryDto>())
                    .Where(s => s.Id.GetValueOrDefault() > 0)
                    .Select(s => s.Id!.Value)
                    .ToHashSet();

                var incomingCategoryIds = categories
                    .Where(c => c.Id.GetValueOrDefault() > 0)
                    .Select(c => c.Id!.Value)
                    .ToHashSet();

                // Identify stale records to delete
                var activitiesToDelete = existingActivities.Where(a => !incomingActivityIds.Contains(a.Id)).ToList();
                var subCategoriesToDelete = existingSubCategories.Where(s => !incomingSubCategoryIds.Contains(s.Id)).ToList();
                var categoriesToDelete = existingCategories.Where(c => !incomingCategoryIds.Contains(c.Id)).ToList();

                var activityIdsToDelete = activitiesToDelete.Select(a => a.Id).ToHashSet();
                var subCategoryIdsToDelete = subCategoriesToDelete.Select(s => s.Id).ToHashSet();
                var categoryIdsToDelete = categoriesToDelete.Select(c => c.Id).ToHashSet();

                // Null out FK references in dependent tables before deleting
                var affectedVendorCategories = await _context.VendorCategories
                    .Where(vc =>
                        (vc.ActivityId.HasValue && activityIdsToDelete.Contains(vc.ActivityId.Value)) ||
                        (vc.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(vc.SubCategoryId.Value)) ||
                        (vc.CategoryId.HasValue && categoryIdsToDelete.Contains(vc.CategoryId.Value)))
                    .ToListAsync();

                foreach (var vc in affectedVendorCategories)
                {
                    if (vc.ActivityId.HasValue && activityIdsToDelete.Contains(vc.ActivityId.Value))
                        vc.ActivityId = null;
                    if (vc.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(vc.SubCategoryId.Value))
                        vc.SubCategoryId = null;
                    if (vc.CategoryId.HasValue && categoryIdsToDelete.Contains(vc.CategoryId.Value))
                        vc.CategoryId = null;
                }

                var affectedTenderCategoryCodes = await _context.TenderCategoryCodes
                    .Where(tc =>
                        (tc.ActivityId.HasValue && activityIdsToDelete.Contains(tc.ActivityId.Value)) ||
                        (tc.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(tc.SubCategoryId.Value)) ||
                        (tc.CategoryId.HasValue && categoryIdsToDelete.Contains(tc.CategoryId.Value)))
                    .ToListAsync();

                foreach (var tc in affectedTenderCategoryCodes)
                {
                    if (tc.ActivityId.HasValue && activityIdsToDelete.Contains(tc.ActivityId.Value))
                        tc.ActivityId = null;
                    if (tc.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(tc.SubCategoryId.Value))
                        tc.SubCategoryId = null;
                    if (tc.CategoryId.HasValue && categoryIdsToDelete.Contains(tc.CategoryId.Value))
                        tc.CategoryId = null;
                }

                var affectedApprovalItems = await _context.CategoryCodeApprovalItems
                    .Where(ai =>
                        (ai.ActivityId.HasValue && activityIdsToDelete.Contains(ai.ActivityId.Value)) ||
                        (ai.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(ai.SubCategoryId.Value)) ||
                        (ai.CategoryId.HasValue && categoryIdsToDelete.Contains(ai.CategoryId.Value)))
                    .ToListAsync();

                foreach (var ai in affectedApprovalItems)
                {
                    if (ai.ActivityId.HasValue && activityIdsToDelete.Contains(ai.ActivityId.Value))
                        ai.ActivityId = null;
                    if (ai.SubCategoryId.HasValue && subCategoryIdsToDelete.Contains(ai.SubCategoryId.Value))
                        ai.SubCategoryId = null;
                    if (ai.CategoryId.HasValue && categoryIdsToDelete.Contains(ai.CategoryId.Value))
                        ai.CategoryId = null;
                }

                // Remove stale records (order: Activities → SubCategories → Categories)
                _context.Activities.RemoveRange(activitiesToDelete);
                _context.SubCategories.RemoveRange(subCategoriesToDelete);
                _context.Categories.RemoveRange(categoriesToDelete);

                await _context.SaveChangesAsync();

                // Upsert hierarchy (id > 0 = update existing, 0 or null = insert new)
                foreach (var cat in categories)
                {
                    Category category;

                    if (cat.Id.GetValueOrDefault() > 0)
                    {
                        category = existingCategories.First(c => c.Id == cat.Id.Value);
                        category.CodeMasterId = cat.CodeMasterId;
                        category.CategoryName = cat.CategoryName;
                    }
                    else
                    {
                        category = new Category
                        {
                            CodeMasterId = cat.CodeMasterId,
                            CategoryName = cat.CategoryName
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    if (cat.SubCategories != null)
                    {
                        foreach (var sub in cat.SubCategories)
                        {
                            SubCategory subCategory;

                            if (sub.Id.GetValueOrDefault() > 0)
                            {
                                subCategory = existingSubCategories.First(s => s.Id == sub.Id.Value);
                                subCategory.CategoryId = category.Id;
                                subCategory.SubCategoryName = sub.SubCategoryName;
                            }
                            else
                            {
                                subCategory = new SubCategory
                                {
                                    CategoryId = category.Id,
                                    SubCategoryName = sub.SubCategoryName
                                };
                                _context.SubCategories.Add(subCategory);
                                await _context.SaveChangesAsync();
                            }

                            if (sub.Activities != null)
                            {
                                foreach (var act in sub.Activities)
                                {
                                    if (act.Id.GetValueOrDefault() > 0)
                                    {
                                        var existingActivity = existingActivities.First(a => a.Id == act.Id.Value);
                                        existingActivity.SubCategoryId = subCategory.Id;
                                        existingActivity.ActivityName = act.ActivityName;
                                    }
                                    else
                                    {
                                        var activity = new Activity
                                        {
                                            SubCategoryId = subCategory.Id,
                                            ActivityName = act.ActivityName
                                        };
                                        _context.Activities.Add(activity);
                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        public async Task SaveTenderManagementAsync(TenderManagementSaveRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Save Settings
                var existingSetting = await _context.TenderSettings.FirstOrDefaultAsync();

                if (existingSetting == null)
                {
                    var setting = new TenderSetting
                    {
                        MinCapitalRequired = request.TenderSetting.MinCapitalRequired,
                        NegotiationLimit = request.TenderSetting.NegotiationLimit
                    };

                    _context.TenderSettings.Add(setting);
                }
                else
                {
                    existingSetting.MinCapitalRequired = request.TenderSetting.MinCapitalRequired;
                    existingSetting.NegotiationLimit = request.TenderSetting.NegotiationLimit;
                }

                // Clear existing documents
                _context.TenderDocuments.RemoveRange(_context.TenderDocuments);

                // Add new documents
                var documents = request.TenderDocuments.Select(x => new TenderDocument
                {
                    JobCategoryId = x.JobCategoryId,
                    DocumentName = x.DocumentName,
                    RequirementType = x.Requirement
                });

                await _context.TenderDocuments.AddRangeAsync(documents);

                // Clear existing evaluation criteria
                _context.EvaluationCriteria.RemoveRange(_context.EvaluationCriteria);

                // Add new criteria
                var criterias = request.EvaluationCriterias.Select(x => new EvaluationCriteria
                {
                    JobCategoryId = x.JobCategoryId,
                    Specification = x.Specification,
                    WeightagePercent = x.Weightage
                });

                await _context.EvaluationCriteria.AddRangeAsync(criterias);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<TenderManagementResponse> GetTenderManagementAsync()
        {
            var setting = await _context.TenderSettings
                .Select(x => new TenderSettingDto
                {
                    MinCapitalRequired = x.MinCapitalRequired,
                    NegotiationLimit = x.NegotiationLimit
                })
                .FirstOrDefaultAsync() ?? null;

            var documents = await _context.TenderDocuments.Include(x => x.JobCategory)
                .Select(x => new TenderDocumentDto
                {
                    JobCategory = x.JobCategory,
                    DocumentName = x.DocumentName,
                    Requirement = x.RequirementType
                })
                .ToListAsync();

            var criterias = await _context.EvaluationCriteria.Include(x => x.JobCategory)
                .Select(x => new EvaluationCriteriaDto
                {
                    JobCategory = x.JobCategory,
                    Specification = x.Specification,
                    Weightage = x.WeightagePercent
                })
                .ToListAsync();

            return new TenderManagementResponse
            {
                TenderSetting = setting,
                TenderDocuments = documents,
                EvaluationCriterias = criterias
            };
        }




        public async Task AddMaterilBudgetAsync(MaterialBudgetDto model)
        {
            var materialBudget = new MaterialBudget
            {
                JobCategoryId = model.JobCategoryId,
                ServiceCode = model.ServiceCode,
                ShortText = model.ShortText,
                MaterialGroup = model.MaterialGroup,
                MaterialGroupDescription = model.MaterialGroupDescription,
                Unit = model.Unit,
                POActAssign = model.POActAssign,
                GLAccount = model.GLAccount,
                GLDescription = model.GLDescription,
                WBSReference = model.WBSReference,
                CostCentreReference = model.CostCentreReference,
                IOReference = model.IOReference,
                Amount = model.Amount
            };
            _context.MaterialBudgets.Add(materialBudget);
            await _context.SaveChangesAsync();
            
        }

        // UPDATE
        public async Task UpdateMaterilBudgetAsync(MaterialBudgetDto model)
        {
            var existing = await _context.MaterialBudgets.FindAsync(model.Id);
            if (existing != null)
            {
                existing.JobCategoryId = model.JobCategoryId;
                existing.ServiceCode = model.ServiceCode;
                existing.ShortText = model.ShortText;
                existing.MaterialGroup = model.MaterialGroup;
                existing.MaterialGroupDescription = model.MaterialGroupDescription;
                existing.Unit = model.Unit;
                existing.POActAssign = model.POActAssign;
                existing.GLAccount = model.GLAccount;
                existing.GLDescription = model.GLDescription;
                existing.WBSReference = model.WBSReference;
                existing.CostCentreReference = model.CostCentreReference;
                existing.IOReference = model.IOReference;
                existing.Amount = model.Amount;
                await _context.SaveChangesAsync();
            }
        }

        // DELETE (Soft Delete Recommended)
        public async Task<bool> DeleteMaterilBudgetAsync(int id)
        {
            var entity = await _context.MaterialBudgets.FindAsync(id);

            if (entity == null)
                return false;

            entity.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // GET BY ID
        public async Task<MaterialBudgetDto> GetMaterilBudgetByIdAsync(int id)
        {
            var materialBudget= await _context.MaterialBudgets
                .Include(x => x.JobCategory)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return _mapper.Map<MaterialBudgetDto>(materialBudget);
        }

        public async Task<IEnumerable<MaterialBudgetDto>> GetAllMaterilBudgetListAsync()
        {
            var materialBudget = await _context.MaterialBudgets.Where(x=>x.IsActive)
                .Include(x => x.JobCategory)
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaterialBudgetDto>>(materialBudget);
        }

        // GET BY JOBCATEGORY
        public async Task<IEnumerable<MaterialBudgetDto>> GetByJobCategoryAsync(int jobCategoryId)
        {
            var materialBudget=  await _context.MaterialBudgets
                .Where(x => x.JobCategoryId == jobCategoryId && x.IsActive)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaterialBudgetDto>>(materialBudget);
        }


        public async Task UploadMaterialBudgetFilesAsync(IEnumerable<IFormFile> files, int uploadedBy)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("No files were uploaded.");

            var uploadRoot = _configuration["FileSettings:UploadPath"];
            if (string.IsNullOrWhiteSpace(uploadRoot))
            {
                uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            }

            var materialFolder = Path.Combine(uploadRoot, "materialbudgets", "budgetuploads");

            if (!Directory.Exists(materialFolder))
                Directory.CreateDirectory(materialFolder);

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                var safeFileName = Path.GetFileName(file.FileName);

                // Step 1: Create DB record first
                var entity = new MaterialBudgetUpload
                {
                    FileName = safeFileName,
                    UploadDateTime = DateTime.UtcNow,
                    CreatedBy = uploadedBy.ToString()??"1"
                };

                _context.MaterialBudgetUploads.Add(entity);
                await _context.SaveChangesAsync(); // generates Id

                // Step 2: Create folder with BudgetUploadId
                var budgetFolder = Path.Combine(materialFolder, entity.Id.ToString());

                if (!Directory.Exists(budgetFolder))
                    Directory.CreateDirectory(budgetFolder);

                // Step 3: Save file
                var fullPath = Path.Combine(budgetFolder, safeFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Step 4: Save relative path
                entity.FilePath = Path.GetRelativePath(uploadRoot, fullPath).Replace("\\", "/");

                _context.MaterialBudgetUploads.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        

        public async Task<IEnumerable<MaterialBudgetUploadDto>> GetMaterialBudgetUploadsAsync()
        {
            var uploads = await _context.MaterialBudgetUploads
                
                .ToListAsync();

            return _mapper.Map<IEnumerable<MaterialBudgetUploadDto>>(uploads);
        }


        public async Task SaveOrUpdateVendorManagementSettingAsync(VendorManagementSettingDto model)
        {
            var existing = await _context.VendorManagementSettings.FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(model.CertificateBackgroundImagePath))
            {
                var uploadRoot = _configuration["FileSettings:UploadPath"];

                var folderPath = Path.Combine(uploadRoot, "VendorManagementSetting");

                // Create directory if not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, "VendorManagementSetting.pdf");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                SaveBase64ToFile(model.CertificateBackgroundImagePath, filePath);

                // Save relative path instead of full path
                model.CertificateBackgroundImagePath = Path
                    .GetRelativePath(uploadRoot, filePath)
                    .Replace("\\", "/");
            }

            if (existing == null)
            {
                // First time save
                var entity = _mapper.Map<VendorManagementSetting>(model);
                entity.CreatedDate = DateTime.UtcNow;

                await _context.VendorManagementSettings.AddAsync(entity);
            }
            else
            {
                // Update existing record
                existing.LateRenewalFee = model.LateRenewalFee;
                existing.BlacklistDenyDurationMonths = model.BlacklistDenyDurationMonths;
                existing.CategoryCodeChangeFee = model.CategoryCodeChangeFee;
                existing.CertificateBackgroundImagePath = model.CertificateBackgroundImagePath;
                existing.RegistrationFee = model.RegistrationFee;
                existing.RenewalFee = model.RenewalFee;
                existing.LateRenewalFee = model.LateRenewalFee;
                existing.UpdatedDate = DateTime.UtcNow;
                existing.PurchaseFee= model.PurchaseFee;
            }

            await _context.SaveChangesAsync();
        }

        public void SaveBase64ToFile(string base64String, string filePath)
        {
            // Remove data:image/png;base64, if exists
            if (base64String.Contains(","))
                base64String = base64String.Substring(base64String.IndexOf(",") + 1);

            byte[] fileBytes = Convert.FromBase64String(base64String);

            File.WriteAllBytes(filePath, fileBytes);
        }

        public async Task<VendorManagementSettingDto> GetVendorManagementSettingAsync()
        {
            var entity = await _context.VendorManagementSettings
                .FirstOrDefaultAsync();

            if (entity == null)
                return null;

            return _mapper.Map<VendorManagementSettingDto>(entity);
        }

        public async Task<(int monthsSetting, int yearsetting)> GetCategoryCodeSettingAsync()
        {
            var setting= await _context.CategoryCodeSetting.FirstOrDefaultAsync();
            if(setting == null)
                return (0, 0);
            int months = setting.EditCategoryCodeAfterMonth;
            int years = setting.EditCategoryCodeLimitAfterYear;

            return (months, years);
        }

        public async Task<IEnumerable<BankKeyDto>> GetAllBankKeysAsync()
        {
            var bankKeys = await _context.BankKeys
                .AsNoTracking()
                .OrderBy(b => b.BankName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BankKeyDto>>(bankKeys);
        }
    }
}