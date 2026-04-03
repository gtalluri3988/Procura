using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class TenderRepository : RepositoryBase<User, UserDTO>, ITenderRepository

    {
        public TenderRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }


        public async Task<int> AddTenderApplicationAsync(TenderApplicationDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = new TenderApplication
                {
                    ApplicationLevelId = dto.ApplicationLevelId,
                    ProjectName = dto.ProjectName,
                    JobCategoryId = dto.JobCategoryId,
                    TenderCategoryId = dto.TenderCategoryId,
                    EstimatedJobStartDate = dto.EstimatedJobStartDate,
                    DepositRequired = dto.DepositRequired,
                    DepositAmount = dto.DepositAmount,
                    Remarks = dto.Remarks,
                    Status = dto.Status,
                    EstimatedPrices = dto.EstimatedPrices,
                    MinCapitalPercent = dto.MinCapitalPercent,
                    MinCapitalAmount = dto.MinCapitalAmount,
                    CreatedDate = DateTime.UtcNow,
                    TenderApplicationStatusId = (int)TenderApplicationStatusEnum.New,
                    TenderCreatedBy = Convert.ToInt32(GetCurrentUserId())
                };

                // Job scopes
                if (dto.JobScopes != null && dto.JobScopes.Any())
                {
                    entity.JobScopes = dto.JobScopes.Select(js => new TenderJobScope
                    {
                        IOWBS = js.IOWBS,
                        MaterialGroup = js.MaterialGroup,
                        MaterialGroupDescription = js.MaterialGroupDescription,
                        ServiceCode = js.ServiceCode,
                        ShortText = js.ShortText,
                        Budget = js.Budget,
                        Unit = js.Unit,
                        Quantity = js.Quantity,
                        PricePerUnit = js.PricePerUnit,
                        SubTotal = js.SubTotal,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();
                }

                // TenderCategoryCodes
                if (dto.TendorCategoryCodeDto != null && dto.TendorCategoryCodeDto.Any())
                {
                    entity.TenderCategoryCode = null; // keep single nav null; we'll add codes collection (TenderApplication has TenderCategoryCode property singular in model)
                                                      // The DB model uses TenderCategoryCode (singular) on TenderApplication; if you need a collection change accordingly.
                                                      // We'll add individual TenderCategoryCode rows as separate entities and link via TenderId.
                }

                // Documents
                if (dto.Documents != null && dto.Documents.Any())
                {
                    entity.Documents = dto.Documents.Select(d => new TenderRequiredDocument
                    {
                        DocumentName = d.DocumentName,
                        Requirement = d.Requirement,
                        Submission = d.Submission,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();
                }

                // Site visit
                if (dto.TenderSiteVisit != null)
                {






                    entity.TenderSiteVisit = new TenderSiteVisit
                    {
                        SiteVisitRequired = dto.TenderSiteVisit.SiteVisitRequired,
                        VisitDate = dto.TenderSiteVisit.VisitDate,
                        Venue = dto.TenderSiteVisit.Venue,
                        Attendance = dto.TenderSiteVisit.Attendance,
                        Remarks = dto.TenderSiteVisit.Remarks,
                        FormFile = dto.TenderSiteVisit.FormFile,
                        CreatedDate = DateTime.UtcNow
                    };
                }

                await _context.Set<TenderApplication>().AddAsync(entity);
                await _context.SaveChangesAsync();

                // If incoming category codes provided, insert them now referencing the generated Id
                if (dto.TendorCategoryCodeDto != null && dto.TendorCategoryCodeDto.Any())
                {
                    var codes = dto.TendorCategoryCodeDto.Select(c => new TenderCategoryCode
                    {
                        TenderId = entity.Id,
                        CodeMasterId = c.CodeMasterId ?? 0,
                        CategoryId = c.CategoryId,
                        SubCategoryId = c.SubCategoryId,
                        ActivityId = c.ActivityId,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();

                    if (codes.Any())
                    {
                        await _context.Set<TenderCategoryCode>().AddRangeAsync(codes);
                        await _context.SaveChangesAsync();
                    }
                }

                await tx.CommitAsync();
                return entity.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<TenderApplicationDto> UpdateTenderApplicationAsync(TenderApplicationDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.Set<TenderApplication>()
                    .Include(t => t.JobScopes)
                    .Include(t => t.Documents)
                    .Include(t => t.TenderSiteVisit)
                    .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (existing == null)
                    throw new Exception("Tender application not found");

                // Update scalar props
                _context.Entry(existing).CurrentValues.SetValues(new
                {
                    dto.ApplicationLevelId,
                    dto.ProjectName,
                    dto.JobCategoryId,
                    dto.TenderCategoryId,
                    dto.EstimatedJobStartDate,
                    dto.DepositRequired,
                    dto.DepositAmount,
                    dto.Remarks,
                    dto.Status,
                    dto.EstimatedPrices,
                    dto.MinCapitalPercent,
                    dto.MinCapitalAmount,
                    UpdatedDate = DateTime.UtcNow
                });

                // Synchronize JobScopes
                var existingScopes = existing.JobScopes?.ToList() ?? new List<TenderJobScope>();
                var incomingScopes = dto.JobScopes ?? new List<TenderJobScope>();

                foreach (var inc in incomingScopes)
                {
                    inc.TenderApplicationId = existing.Id;
                    if (inc.Id > 0)
                    {
                        var ex = existingScopes.FirstOrDefault(x => x.Id == inc.Id);
                        if (ex != null)
                        {
                            _context.Entry(ex).CurrentValues.SetValues(inc);
                        }
                        else
                        {
                            // id provided but not found -> treat as new
                            await _context.Set<TenderJobScope>().AddAsync(new TenderJobScope
                            {
                                TenderApplicationId = existing.Id,
                                IOWBS = inc.IOWBS,
                                MaterialGroup = inc.MaterialGroup,
                                MaterialGroupDescription = inc.MaterialGroupDescription,
                                ServiceCode = inc.ServiceCode,
                                ShortText = inc.ShortText,
                                Budget = inc.Budget,
                                Unit = inc.Unit,
                                Quantity = inc.Quantity,
                                PricePerUnit = inc.PricePerUnit,
                                SubTotal = inc.SubTotal,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }
                    else
                    {
                        await _context.Set<TenderJobScope>().AddAsync(new TenderJobScope
                        {
                            TenderApplicationId = existing.Id,
                            IOWBS = inc.IOWBS,
                            MaterialGroup = inc.MaterialGroup,
                            MaterialGroupDescription = inc.MaterialGroupDescription,
                            ServiceCode = inc.ServiceCode,
                            ShortText = inc.ShortText,
                            Budget = inc.Budget,
                            Unit = inc.Unit,
                            Quantity = inc.Quantity,
                            PricePerUnit = inc.PricePerUnit,
                            SubTotal = inc.SubTotal,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }

                var incomingScopeIds = incomingScopes.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();
                var toRemoveScopes = existingScopes.Where(e => !incomingScopeIds.Contains(e.Id)).ToList();
                if (toRemoveScopes.Any()) _context.Set<TenderJobScope>().RemoveRange(toRemoveScopes);

                // Synchronize Documents
                var existingDocs = existing.Documents?.ToList() ?? new List<TenderRequiredDocument>();
                var incomingDocs = dto.Documents ?? new List<TenderRequiredDocument>();

                foreach (var inc in incomingDocs)
                {
                    inc.TenderApplicationId = existing.Id;
                    if (inc.Id > 0)
                    {
                        var ex = existingDocs.FirstOrDefault(x => x.Id == inc.Id);
                        if (ex != null)
                        {
                            _context.Entry(ex).CurrentValues.SetValues(new
                            {
                                inc.DocumentName,
                                inc.Requirement,
                                inc.Submission,
                                UpdatedDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            await _context.Set<TenderRequiredDocument>().AddAsync(new TenderRequiredDocument
                            {
                                TenderApplicationId = existing.Id,
                                DocumentName = inc.DocumentName,
                                Requirement = inc.Requirement,
                                Submission = inc.Submission,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }
                    else
                    {
                        await _context.Set<TenderRequiredDocument>().AddAsync(new TenderRequiredDocument
                        {
                            TenderApplicationId = existing.Id,
                            DocumentName = inc.DocumentName,
                            Requirement = inc.Requirement,
                            Submission = inc.Submission,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }

                var incomingDocIds = incomingDocs.Where(d => d.Id > 0).Select(d => d.Id).ToHashSet();
                var toRemoveDocs = existingDocs.Where(e => !incomingDocIds.Contains(e.Id)).ToList();
                if (toRemoveDocs.Any()) _context.Set<TenderRequiredDocument>().RemoveRange(toRemoveDocs);

                // Site visit
                if (dto.TenderSiteVisit != null)
                {
                    if (existing.TenderSiteVisit == null)
                    {
                        existing.TenderSiteVisit = new TenderSiteVisit
                        {
                            TenderId = existing.Id,
                            SiteVisitRequired = dto.TenderSiteVisit.SiteVisitRequired,
                            VisitDate = dto.TenderSiteVisit.VisitDate,
                            Venue = dto.TenderSiteVisit.Venue,
                            Attendance = dto.TenderSiteVisit.Attendance,
                            Remarks = dto.TenderSiteVisit.Remarks,
                            FormFile = dto.TenderSiteVisit.FormFile,
                            CreatedDate = DateTime.UtcNow
                        };
                        _context.Set<TenderSiteVisit>().Add(existing.TenderSiteVisit);
                    }
                    else
                    {
                        _context.Entry(existing.TenderSiteVisit).CurrentValues.SetValues(new
                        {
                            dto.TenderSiteVisit.SiteVisitRequired,
                            dto.TenderSiteVisit.VisitDate,
                            dto.TenderSiteVisit.Venue,
                            dto.TenderSiteVisit.Attendance,
                            dto.TenderSiteVisit.Remarks,
                            dto.TenderSiteVisit.FormFile,
                            UpdatedDate = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    if (existing.TenderSiteVisit != null)
                        _context.Set<TenderSiteVisit>().Remove(existing.TenderSiteVisit);
                }

                // TenderCategoryCode sync (incoming DTO -> TenderCategoryCode rows)
                if (dto.TendorCategoryCodeDto != null)
                {
                    var existingCodes = await _context.Set<TenderCategoryCode>().Where(c => c.TenderId == existing.Id).ToListAsync();
                    foreach (var inc in dto.TendorCategoryCodeDto)
                    {
                        if (inc.Id > 0)
                        {
                            var ex = existingCodes.FirstOrDefault(x => x.Id == inc.Id);
                            if (ex != null)
                            {
                                _context.Entry(ex).CurrentValues.SetValues(new
                                {
                                    CodeMasterId = inc.CodeMasterId ?? ex.CodeMasterId,
                                    CategoryId = inc.CategoryId,
                                    SubCategoryId = inc.SubCategoryId,
                                    ActivityId = inc.ActivityId,
                                    UpdatedDate = DateTime.UtcNow
                                });
                            }
                            else
                            {
                                await _context.Set<TenderCategoryCode>().AddAsync(new TenderCategoryCode
                                {
                                    TenderId = existing.Id,
                                    CodeMasterId = inc.CodeMasterId ?? 0,
                                    CategoryId = inc.CategoryId,
                                    SubCategoryId = inc.SubCategoryId,
                                    ActivityId = inc.ActivityId,
                                    CreatedDate = DateTime.UtcNow
                                });
                            }
                        }
                        else
                        {
                            await _context.Set<TenderCategoryCode>().AddAsync(new TenderCategoryCode
                            {
                                TenderId = existing.Id,
                                CodeMasterId = inc.CodeMasterId ?? 0,
                                CategoryId = inc.CategoryId,
                                SubCategoryId = inc.SubCategoryId,
                                ActivityId = inc.ActivityId,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }

                    var incomingCodeIds = dto.TendorCategoryCodeDto.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                    var removeCodes = existingCodes.Where(e => !incomingCodeIds.Contains(e.Id)).ToList();
                    if (removeCodes.Any()) _context.Set<TenderCategoryCode>().RemoveRange(removeCodes);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Map back to DTO minimal
                dto.CreatedDate = existing.CreatedDate;
                dto.Id = existing.Id;
                return dto;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteTenderApplicationAsync(int tenderApplicationId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.Set<TenderApplication>()
                    .FirstOrDefaultAsync(t => t.Id == tenderApplicationId);

                if (existing == null) return;

                _context.Set<TenderApplication>().Remove(existing);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync()
        {
            var tenders = await _context.Set<TenderApplication>()
                .Include(t => t.JobScopes)
                .Include(t => t.Documents)
                .Include(t => t.TenderSiteVisit)
                .Include(t => t.TenderCategoryCode)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<TenderApplicationDto>>(tenders);
        }

        public async Task<TenderApplicationDto?> GetTenderApplicationByIdAsync(int id)
        {
            var tender = await _context.Set<TenderApplication>()
                .Include(t => t.JobScopes)
                .Include(t => t.Documents)
                .Include(t => t.TenderSiteVisit)
                .Include(t => t.TenderCategoryCode)
                .Include(t => t.TenderCreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tender == null)
                return null;

            return _mapper.Map<TenderApplicationDto>(tender);
        }

        public async Task<List<TenderApplicationDto>> GetAllTenderApplicationsAsync(
    int? applicationLevelId,
    int? tenderCategoryId,
    int? jobCategoryId,
    int? statusId)
        {
            var query = _context.Set<TenderApplication>()
                .Include(t => t.JobScopes)
                .Include(t => t.Documents)
                .Include(t => t.TenderSiteVisit)
                .Include(t => t.TenderCategoryCode)
                .AsQueryable();

            // Application Level Filter
            if (applicationLevelId.HasValue)
            {
                query = query.Where(t => t.ApplicationLevelId == applicationLevelId);
            }

            // Tender Category Filter
            if (tenderCategoryId.HasValue)
            {
                query = query.Where(t => t.TenderCategoryId == tenderCategoryId);
            }

            // Job Category Filter
            if (jobCategoryId.HasValue)
            {
                query = query.Where(t => t.JobCategoryId == jobCategoryId);
            }

            // Status Filter
            if (statusId.HasValue)
            {
                query = query.Where(t => t.TenderApplicationStatusId == statusId);
            }

            var tenders = await query
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<TenderApplicationDto>>(tenders);
        }

        public async Task<IEnumerable<UserDTO>> GetTendorReviewers(int ApplicationLevelId, int DesignationId, int StateId)
        {
            try
            {
                var Users = await _context.Users.Where(x=>x.SiteLevelId== ApplicationLevelId
                && x.DesignationId== DesignationId && x.SiteOffice==StateId).ToListAsync();
                return _mapper.Map<IEnumerable<UserDTO>>(Users);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrive record");
            }
        }

        public async Task SaveTenderAdvertisementPageAsync(TenderAdvertisementPageDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                int tenderId = dto.TenderApplicationId;

                // --- Advertisement Setting (upsert) ---
                if (dto.AdvertisementSetting != null)
                {
                    var s = dto.AdvertisementSetting;
                    var existing = await _context.Set<TenderAdvertisementSetting>()
                        .FirstOrDefaultAsync(x => x.TenderId == tenderId);

                    if (existing == null)
                    {
                        await _context.Set<TenderAdvertisementSetting>().AddAsync(new TenderAdvertisementSetting
                        {
                            TenderId = tenderId,
                            AdvertisementStartDate = s.AdvertisementStartDate,
                            AdvertisementEndDate = s.AdvertisementEndDate,
                            EndTime = s.EndTime,
                            OpeningStartDate = s.OpeningStartDate,
                            OpeningEndDate = s.OpeningEndDate,
                            EvaluationStartDate = s.EvaluationStartDate,
                            EvaluationEndDate = s.EvaluationEndDate,
                            TenderDocumentPath = s.TenderDocumentPath,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        existing.AdvertisementStartDate = s.AdvertisementStartDate;
                        existing.AdvertisementEndDate = s.AdvertisementEndDate;
                        existing.EndTime = s.EndTime;
                        existing.OpeningStartDate = s.OpeningStartDate;
                        existing.OpeningEndDate = s.OpeningEndDate;
                        existing.EvaluationStartDate = s.EvaluationStartDate;
                        existing.EvaluationEndDate = s.EvaluationEndDate;
                        existing.TenderDocumentPath = s.TenderDocumentPath;
                        existing.UpdatedDate = DateTime.UtcNow;
                    }
                }

                // --- Opening Committee (replace-all sync) ---
                var existingOpening = await _context.Set<TenderOpeningCommittee>()
                    .Where(x => x.TenderId == tenderId).ToListAsync();
                _context.Set<TenderOpeningCommittee>().RemoveRange(existingOpening);

                if (dto.OpeningCommittees != null && dto.OpeningCommittees.Any())
                {
                    var newOpening = dto.OpeningCommittees.Select(m => new TenderOpeningCommittee
                    {
                        TenderId = tenderId,
                        UserId = m.UserId,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();
                    await _context.Set<TenderOpeningCommittee>().AddRangeAsync(newOpening);
                }

                // --- Evaluation Committee (replace-all sync) ---
                var existingEval = await _context.Set<TenderEvaluationCommittee>()
                    .Where(x => x.TenderId == tenderId).ToListAsync();
                _context.Set<TenderEvaluationCommittee>().RemoveRange(existingEval);

                if (dto.EvaluationCommittees != null && dto.EvaluationCommittees.Any())
                {
                    var newEval = dto.EvaluationCommittees.Select(m => new TenderEvaluationCommittee
                    {
                        TenderId = tenderId,
                        UserId = m.UserId,
                        CreatedDate = DateTime.UtcNow
                    }).ToList();
                    await _context.Set<TenderEvaluationCommittee>().AddRangeAsync(newEval);
                }

                // --- Evaluation Criteria + Specifications (upsert by Id, remove missing) ---
                // If no criteria provided, seed initial data from master EvaluationCriteria by JobCategoryId
                if (dto.EvaluationCriterias == null || !dto.EvaluationCriterias.Any())
                {
                    var anyExisting = await _context.Set<TenderEvaluationCriteria>()
                        .AnyAsync(c => c.TenderId == tenderId);

                    if (!anyExisting)
                    {
                        var tenderApp = await _context.Set<TenderApplication>()
                            .FirstOrDefaultAsync(t => t.Id == tenderId);

                        if (tenderApp != null)
                        {
                            var masterCriterias = await _context.Set<EvaluationCriteria>()
                                .Where(ec => ec.JobCategoryId == tenderApp.JobCategoryId && ec.IsActive)
                                .ToListAsync();

                            if (masterCriterias.Any())
                            {
                                var grouped = masterCriterias.GroupBy(ec => ec.JobCategoryId);
                                foreach (var group in grouped)
                                {
                                    var newCriteria = new TenderEvaluationCriteria
                                    {
                                        TenderId = tenderId,
                                        JobCategoryId = group.Key,
                                        IsActive = true,
                                        PassingMarks = 0,
                                        CreatedDate = DateTime.UtcNow,
                                        Specifications = group.Select(ec => new TenderEvaluationSpecification
                                        {
                                            Specification = ec.Specification,
                                            Weightage = ec.WeightagePercent,
                                            CreatedDate = DateTime.UtcNow
                                        }).ToList()
                                    };
                                    await _context.Set<TenderEvaluationCriteria>().AddAsync(newCriteria);
                                }
                            }
                        }
                    }
                }

                if (dto.EvaluationCriterias != null && dto.EvaluationCriterias.Any())
                {
                    var existingCriterias = await _context.Set<TenderEvaluationCriteria>()
                        .Include(c => c.Specifications)
                        .Where(c => c.TenderId == tenderId)
                        .ToListAsync();

                    var incomingIds = dto.EvaluationCriterias.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                    var toRemove = existingCriterias.Where(c => !incomingIds.Contains(c.Id)).ToList();
                    if (toRemove.Any()) _context.Set<TenderEvaluationCriteria>().RemoveRange(toRemove);

                    foreach (var criteriaDto in dto.EvaluationCriterias)
                    {
                        TenderEvaluationCriteria criteria;
                        if (criteriaDto.Id > 0)
                        {
                            criteria = existingCriterias.FirstOrDefault(c => c.Id == criteriaDto.Id)
                                       ?? new TenderEvaluationCriteria { TenderId = tenderId };
                            if (criteria.Id == 0)
                                await _context.Set<TenderEvaluationCriteria>().AddAsync(criteria);
                        }
                        else
                        {
                            criteria = new TenderEvaluationCriteria { TenderId = tenderId, CreatedDate = DateTime.UtcNow };
                            await _context.Set<TenderEvaluationCriteria>().AddAsync(criteria);
                        }

                        criteria.JobCategoryId = criteriaDto.JobCategoryId;
                        criteria.IsActive = criteriaDto.IsActive;
                        criteria.PassingMarks = criteriaDto.PassingMarks;
                        criteria.UpdatedDate = DateTime.UtcNow;

                        // Sync specifications
                        var existingSpecs = criteria.Specifications?.ToList() ?? new List<TenderEvaluationSpecification>();
                        var incomingSpecIds = criteriaDto.Specifications.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();
                        var specsToRemove = existingSpecs.Where(s => !incomingSpecIds.Contains(s.Id)).ToList();
                        if (specsToRemove.Any()) _context.Set<TenderEvaluationSpecification>().RemoveRange(specsToRemove);

                        foreach (var specDto in criteriaDto.Specifications)
                        {
                            if (specDto.Id > 0)
                            {
                                var existingSpec = existingSpecs.FirstOrDefault(s => s.Id == specDto.Id);
                                if (existingSpec != null)
                                {
                                    existingSpec.Specification = specDto.Specification;
                                    existingSpec.Weightage = specDto.Weightage;
                                    existingSpec.UpdatedDate = DateTime.UtcNow;
                                }
                                else
                                {
                                    await _context.Set<TenderEvaluationSpecification>().AddAsync(new TenderEvaluationSpecification
                                    {
                                        CriteriaId = criteria.Id,
                                        Specification = specDto.Specification,
                                        Weightage = specDto.Weightage,
                                        CreatedDate = DateTime.UtcNow
                                    });
                                }
                            }
                            else
                            {
                                criteria.Specifications ??= new List<TenderEvaluationSpecification>();
                                criteria.Specifications.Add(new TenderEvaluationSpecification
                                {
                                    Specification = specDto.Specification,
                                    Weightage = specDto.Weightage,
                                    CreatedDate = DateTime.UtcNow
                                });
                            }
                        }
                    }
                }

                // --- Issuance Approvals (upsert by ReviewLevel, remove missing levels) ---
                if (dto.IssuanceApprovals != null)
                {
                    var existingApprovals = await _context.Set<TenderIssuenceApproval>()
                        .Where(a => a.TenderId == tenderId).ToListAsync();

                    var incomingLevels = dto.IssuanceApprovals.Select(a => a.ReviewLevel).ToHashSet();
                    var toRemoveApprovals = existingApprovals.Where(a => !incomingLevels.Contains(a.ReviewLevel)).ToList();
                    if (toRemoveApprovals.Any()) _context.Set<TenderIssuenceApproval>().RemoveRange(toRemoveApprovals);

                    foreach (var approvalDto in dto.IssuanceApprovals)
                    {
                        var existing = existingApprovals.FirstOrDefault(a => a.ReviewLevel == approvalDto.ReviewLevel);
                        if (existing == null)
                        {
                            await _context.Set<TenderIssuenceApproval>().AddAsync(new TenderIssuenceApproval
                            {
                                TenderId = tenderId,
                                ReviewLevel = approvalDto.ReviewLevel,
                                ReviewerId = approvalDto.ReviewerId,
                                TenderStatusId = approvalDto.TenderStatusId,
                                Remarks = approvalDto.Remarks,
                                ReviewDateTime = approvalDto.ReviewDateTime,
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            existing.ReviewerId = approvalDto.ReviewerId;
                            existing.TenderStatusId = approvalDto.TenderStatusId;
                            existing.Remarks = approvalDto.Remarks;
                            existing.ReviewDateTime = approvalDto.ReviewDateTime;
                            existing.UpdatedDate = DateTime.UtcNow;
                        }
                    }
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

        public async Task<TenderAdvertisementPageDto?> GetTenderAdvertisementPageByTenderApplicationIdAsync(int tenderApplicationId)
        {
            var tenderExists = await _context.Set<TenderApplication>()
                .AnyAsync(t => t.Id == tenderApplicationId);

            if (!tenderExists)
                return null;

            var setting = await _context.Set<TenderAdvertisementSetting>()
                .FirstOrDefaultAsync(x => x.TenderId == tenderApplicationId);

            var openingCommittees = await _context.Set<TenderOpeningCommittee>()
                .Include(x => x.User)
                .Where(x => x.TenderId == tenderApplicationId)
                .ToListAsync();

            var evaluationCommittees = await _context.Set<TenderEvaluationCommittee>()
                .Include(x => x.User)
                .Where(x => x.TenderId == tenderApplicationId)
                .ToListAsync();

            var criterias = await _context.Set<TenderEvaluationCriteria>()
                .Include(c => c.Specifications)
                .Where(c => c.TenderId == tenderApplicationId)
                .ToListAsync();

            var approvals = await _context.Set<TenderIssuenceApproval>()
                .Include(a => a.Reviewer)
                .Where(a => a.TenderId == tenderApplicationId)
                .OrderBy(a => a.ReviewLevel)
                .ToListAsync();

            return new TenderAdvertisementPageDto
            {
                TenderApplicationId = tenderApplicationId,

                AdvertisementSetting = setting == null ? null : new TenderAdvertisementSettingDto
                {
                    Id = setting.Id,
                    TenderApplicationId = setting.TenderId,
                    AdvertisementStartDate = setting.AdvertisementStartDate,
                    AdvertisementEndDate = setting.AdvertisementEndDate,
                    EndTime = setting.EndTime,
                    OpeningStartDate = setting.OpeningStartDate,
                    OpeningEndDate = setting.OpeningEndDate,
                    EvaluationStartDate = setting.EvaluationStartDate,
                    EvaluationEndDate = setting.EvaluationEndDate,
                    TenderDocumentPath = setting.TenderDocumentPath
                },

                OpeningCommittees = openingCommittees.Select(m => new TenderOpeningCommitteeDto
                {
                    Id = m.Id,
                    TenderApplicationId = m.TenderId,
                    UserId = m.UserId,
                    UserName = m.User?.UserName,
                    FullName = m.User?.FullName
                }).ToList(),

                EvaluationCommittees = evaluationCommittees.Select(m => new TenderEvaluationCommitteeDto
                {
                    Id = m.Id,
                    TenderApplicationId = m.TenderId,
                    UserId = m.UserId,
                    UserName = m.User?.UserName,
                    FullName = m.User?.FullName
                }).ToList(),

                EvaluationCriterias = criterias.Select(c => new TenderEvaluationCriteriaDto
                {
                    Id = c.Id,
                    TenderApplicationId = c.TenderId,
                    JobCategoryId = c.JobCategoryId,
                    IsActive = c.IsActive,
                    PassingMarks = c.PassingMarks,
                    Specifications = (c.Specifications ?? new List<TenderEvaluationSpecification>())
                        .Select(s => new TenderEvaluationSpecificationDto
                        {
                            Id = s.Id,
                            Specification = s.Specification,
                            Weightage = s.Weightage
                        }).ToList()
                }).ToList(),

                IssuanceApprovals = approvals.Select(a => new TenderIssuenceApprovalDto
                {
                    Id = a.Id,
                    TenderApplicationId = a.TenderId,
                    ReviewLevel = a.ReviewLevel,
                    ReviewerId = a.ReviewerId,
                    TenderStatusId = a.TenderStatusId,
                    Remarks = a.Remarks,
                    ReviewDateTime = a.ReviewDateTime,
                    ReviewerName = a.Reviewer?.FullName
                }).ToList()
            };
        }
        public async Task<IEnumerable<UserDTO>> SearchCommitteeUsersAsync(string name, string committeeType)
        {
            var query = _context.Users.Where(u => u.IsActive);

            if (committeeType.Equals("opening", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.IsOpeningCommittee);
            else if (committeeType.Equals("evaluation", StringComparison.OrdinalIgnoreCase))
                query = query.Where(u => u.IsEvaluationCommittee);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.FullName.Contains(name));

            var users = await query.ToListAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        // ── Tender Opening — Search List ─────────────────────────────────────────
        public async Task<List<TenderOpeningListDto>> GetTenderOpeningListAsync(string? referenceId, string? projectName)
        {
            var query = _context.Set<TenderApplication>()
                .Include(t => t.TenderApplicationStatus)
                .AsQueryable();

            // Filter by reference ID: strip leading "T" and parse
            if (!string.IsNullOrWhiteSpace(referenceId))
            {
                var clean = referenceId.TrimStart('T', 't').Trim();
                if (int.TryParse(clean, out int refNum))
                    query = query.Where(t => t.Id == refNum);
            }

            if (!string.IsNullOrWhiteSpace(projectName))
                query = query.Where(t => t.ProjectName.Contains(projectName));

            var tenders = await query.OrderByDescending(t => t.CreatedDate).ToListAsync();

            // Batch-load advertisement settings to avoid N+1
            var tenderIds = tenders.Select(t => t.Id).ToList();
            var adSettings = await _context.Set<TenderAdvertisementSetting>()
                .Where(a => tenderIds.Contains(a.TenderId))
                .ToDictionaryAsync(a => a.TenderId);

            return tenders.Select(t =>
            {
                adSettings.TryGetValue(t.Id, out var ad);
                return new TenderOpeningListDto
                {
                    Id = t.Id,
                    ReferenceId = "T" + t.Id.ToString("D7"),
                    ProjectName = t.ProjectName,
                    StartDate = ad?.AdvertisementStartDate,
                    EndDate = ad?.AdvertisementEndDate,
                    TenderStatus = t.TenderApplicationStatus?.Name ?? t.Status
                };
            }).ToList();
        }

        // ── Tender Opening — Detail (Page 2) ─────────────────────────────────────
        public async Task<TenderOpeningDetailDto?> GetTenderOpeningDetailAsync(int tenderId)
        {
            var tender = await _context.Set<TenderApplication>()
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var ad = await _context.Set<TenderAdvertisementSetting>()
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            return new TenderOpeningDetailDto
            {
                Id = tender.Id,
                ReferenceId = "T" + tender.Id.ToString("D7"),
                ProjectName = tender.ProjectName,
                StartDate = ad?.AdvertisementStartDate,
                EndDate = ad?.AdvertisementEndDate
            };
        }

        // ── Tender Evaluation — Full Page ────────────────────────────────────────
        public async Task<TenderEvaluationPageDto?> GetTenderEvaluationPageAsync(int tenderId)
        {
            var tender = await _context.Set<TenderApplication>()
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            // All vendor submissions for this tender, ordered by sequence
            var submissions = await _context.Set<TenderVendorSubmission>()
                .Where(s => s.TenderId == tenderId)
                .OrderBy(s => s.SequenceNo)
                .ToListAsync();

            int totalVendors = submissions.Count;

            // Technical results keyed by vendorId
            var techResults = await _context.Set<TenderTechnicalEvaluationResult>()
                .Where(r => r.TenderId == tenderId)
                .ToDictionaryAsync(r => r.VendorId);

            // Financial data keyed by vendorId
            var vendorIds = submissions.Select(s => s.VendorId).ToList();
            var financials = await _context.Set<VendorFinancial>()
                .Where(f => vendorIds.Contains(f.VendorId))
                .ToDictionaryAsync(f => f.VendorId);

            decimal minCapital = tender.MinCapitalAmount ?? 0m;

            // Technical rows
            var technicalRows = submissions.Select(s =>
            {
                techResults.TryGetValue(s.VendorId, out var tr);
                return new TenderTechnicalEvalRowDto
                {
                    VendorId = s.VendorId,
                    TendererCode = $"{s.SequenceNo}/{totalVendors}",
                    TenderOpeningStatus = s.TenderOpeningStatus,
                    TechnicalEvaluationStatus = tr != null ? tr.Result : "Pending",
                    Result = tr != null ? tr.Result : "Pending",
                    Ranking = tr?.Ranking ?? 0
                };
            }).ToList();

            // Financial rows (auto-computed)
            var financialRows = submissions.Select(s =>
            {
                financials.TryGetValue(s.VendorId, out var fin);
                decimal liquidCapital = fin?.LiquidCapital ?? 0m;
                decimal assetBalance = fin?.AssetBalance ?? 0m;
                decimal finalAmount = Math.Min(liquidCapital, assetBalance);
                string result = finalAmount >= minCapital ? "Passed" : "Failed";

                return new TenderFinancialEvalRowDto
                {
                    VendorId = s.VendorId,
                    TendererCode = $"{s.SequenceNo}/{totalVendors}",
                    CapitalLiquidation = liquidCapital,
                    AssetBalance = assetBalance,
                    FinalAmount = finalAmount,
                    MinCapitalRequired = minCapital,
                    Result = result
                };
            }).ToList();

            // Recommendation
            var savedRec = await _context.Set<TenderRecommendation>()
                .FirstOrDefaultAsync(r => r.TenderId == tenderId);

            var finResultsDict = financialRows.ToDictionary(f => f.VendorId, f => f.Result);

            var recommRows = submissions.Select(s =>
            {
                techResults.TryGetValue(s.VendorId, out var tr);
                finResultsDict.TryGetValue(s.VendorId, out var finResult);
                decimal offerPrice = s.OfferedPrice ?? 0m;
                decimal estimation = tender.EstimatedPrices ?? 0m;
                decimal diffPct = estimation > 0
                    ? Math.Round(((offerPrice - estimation) / estimation) * 100, 2)
                    : 0m;

                return new TenderRecommRowDto
                {
                    VendorId = s.VendorId,
                    TendererCode = $"{s.SequenceNo}/{totalVendors}",
                    TenderOpeningStatus = s.TenderOpeningStatus,
                    TechnicalEvaluationStatus = tr != null ? tr.Result : "Pending",
                    FinancialEvaluationStatus = finResult ?? "Pending",
                    VendorOfferPrice = offerPrice,
                    FpmsEstimationPrice = estimation,
                    DifferencePercent = diffPct
                };
            }).ToList();

            var vendorOptions = submissions.Select(s => new TendererOptionDto
            {
                VendorId = s.VendorId,
                TendererCode = $"{s.SequenceNo}/{totalVendors}"
            }).ToList();

            return new TenderEvaluationPageDto
            {
                TenderId = tender.Id,
                ReferenceId = "T" + tender.Id.ToString("D7"),
                ProjectName = tender.ProjectName,
                TechnicalRows = technicalRows,
                FinancialRows = financialRows,
                Recommendation = new TenderRecommendationPageDto
                {
                    TenderId = tender.Id,
                    RecommendedVendorId = savedRec?.RecommendedVendorId,
                    Reason = savedRec?.Reason,
                    Rows = recommRows,
                    VendorOptions = vendorOptions
                }
            };
        }

        // ── Tender Evaluation — Technical Popup ──────────────────────────────────
        public async Task<TenderTechnicalEvalPopupDto?> GetTechnicalEvaluationPopupAsync(int tenderId, int vendorId)
        {
            var tender = await _context.Set<TenderApplication>()
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            // Get submissions to build tenderer code
            var allSubmissions = await _context.Set<TenderVendorSubmission>()
                .Where(s => s.TenderId == tenderId)
                .OrderBy(s => s.SequenceNo)
                .ToListAsync();

            var submission = allSubmissions.FirstOrDefault(s => s.VendorId == vendorId);
            int totalVendors = allSubmissions.Count;
            string tendererCode = submission != null
                ? $"{submission.SequenceNo}/{totalVendors}"
                : $"?/{totalVendors}";

            // Evaluation criteria + specifications for this tender
            var criteria = await _context.Set<TenderEvaluationCriteria>()
                .Include(c => c.Specifications)
                .FirstOrDefaultAsync(c => c.TenderId == tenderId);

            if (criteria == null) return null;

            var specs = criteria.Specifications?.ToList() ?? new List<TenderEvaluationSpecification>();

            // Existing scores for this vendor
            var specIds = specs.Select(s => s.Id).ToList();
            var existingScores = await _context.Set<TenderTechnicalEvaluationScore>()
                .Where(sc => sc.TenderId == tenderId && sc.VendorId == vendorId && specIds.Contains(sc.SpecificationId))
                .ToDictionaryAsync(sc => sc.SpecificationId);

            var criterionDtos = specs.Select((spec, idx) =>
            {
                existingScores.TryGetValue(spec.Id, out var saved);
                int score = saved?.Score ?? 0;
                int total = spec.Weightage * score / 5;
                return new TechnicalCriterionDto
                {
                    SpecificationId = spec.Id,
                    Specification = spec.Specification ?? string.Empty,
                    Weightage = spec.Weightage,
                    Score = score,
                    Total = total,
                    Remarks = saved?.Remarks
                };
            }).ToList();

            int totalScore = criterionDtos.Sum(c => c.Total);
            int passingMarks = criteria.PassingMarks;
            string result = criterionDtos.Any(c => c.Score > 0)
                ? (totalScore >= passingMarks ? "Passed" : "Failed")
                : "Pending";

            return new TenderTechnicalEvalPopupDto
            {
                TenderId = tenderId,
                ReferenceId = "T" + tender.Id.ToString("D7"),
                ProjectName = tender.ProjectName,
                TendererCode = tendererCode,
                VendorId = vendorId,
                Criteria = criterionDtos,
                TotalScore = totalScore,
                PassingMarks = passingMarks,
                Result = result
            };
        }

        // ── Tender Evaluation — Save Technical Scores ────────────────────────────
        public async Task SaveTechnicalScoreAsync(SaveTechnicalScoreDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                int tenderId = dto.TenderId;
                int vendorId = dto.VendorId;

                // Upsert each criterion score
                foreach (var scoreDto in dto.Scores)
                {
                    var existing = await _context.Set<TenderTechnicalEvaluationScore>()
                        .FirstOrDefaultAsync(s =>
                            s.TenderId == tenderId &&
                            s.VendorId == vendorId &&
                            s.SpecificationId == scoreDto.SpecificationId);

                    if (existing == null)
                    {
                        await _context.Set<TenderTechnicalEvaluationScore>().AddAsync(
                            new TenderTechnicalEvaluationScore
                            {
                                TenderId = tenderId,
                                VendorId = vendorId,
                                SpecificationId = scoreDto.SpecificationId,
                                Score = scoreDto.Score,
                                Remarks = scoreDto.Remarks,
                                CreatedDate = DateTime.UtcNow
                            });
                    }
                    else
                    {
                        existing.Score = scoreDto.Score;
                        existing.Remarks = scoreDto.Remarks;
                        existing.UpdatedDate = DateTime.UtcNow;
                    }
                }

                // Compute totals and upsert result
                var criteria = await _context.Set<TenderEvaluationCriteria>()
                    .FirstOrDefaultAsync(c => c.TenderId == tenderId);

                int passingMarks = criteria?.PassingMarks ?? 0;
                int totalScore = dto.Scores.Sum(s => s.Weightage * s.Score / 5);
                string result = totalScore >= passingMarks ? "Passed" : "Failed";

                var existingResult = await _context.Set<TenderTechnicalEvaluationResult>()
                    .FirstOrDefaultAsync(r => r.TenderId == tenderId && r.VendorId == vendorId);

                if (existingResult == null)
                {
                    await _context.Set<TenderTechnicalEvaluationResult>().AddAsync(
                        new TenderTechnicalEvaluationResult
                        {
                            TenderId = tenderId,
                            VendorId = vendorId,
                            TotalScore = totalScore,
                            PassingMarks = passingMarks,
                            Result = result,
                            Ranking = 0,
                            CreatedDate = DateTime.UtcNow
                        });
                }
                else
                {
                    existingResult.TotalScore = totalScore;
                    existingResult.PassingMarks = passingMarks;
                    existingResult.Result = result;
                    existingResult.UpdatedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Recompute rankings for all vendors on this tender (highest score = rank 1)
                var allResults = await _context.Set<TenderTechnicalEvaluationResult>()
                    .Where(r => r.TenderId == tenderId)
                    .OrderByDescending(r => r.TotalScore)
                    .ToListAsync();

                for (int i = 0; i < allResults.Count; i++)
                {
                    allResults[i].Ranking = i + 1;
                    allResults[i].UpdatedDate = DateTime.UtcNow;
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

        // ── Tender Evaluation — Save Recommendation ──────────────────────────────
        public async Task SaveTenderRecommendationAsync(TenderRecommendationPageDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.Set<TenderRecommendation>()
                    .FirstOrDefaultAsync(r => r.TenderId == dto.TenderId);

                if (existing == null)
                {
                    await _context.Set<TenderRecommendation>().AddAsync(new TenderRecommendation
                    {
                        TenderId = dto.TenderId,
                        RecommendedVendorId = dto.RecommendedVendorId,
                        Reason = dto.Reason,
                        CreatedDate = DateTime.UtcNow
                    });
                }
                else
                {
                    existing.RecommendedVendorId = dto.RecommendedVendorId;
                    existing.Reason = dto.Reason;
                    existing.UpdatedDate = DateTime.UtcNow;
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

        // ── Tender Opening — Full Page (Page 3, after Proceed) ───────────────────
        public async Task<TenderOpeningPageDto?> GetTenderOpeningPageAsync(int tenderId)
        {
            var tender = await _context.Set<TenderApplication>()
                .Include(t => t.TenderCategory)
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var ad = await _context.Set<TenderAdvertisementSetting>()
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            // Category codes — join CodeMaster.Code with " AND "
            var categoryCodes = await _context.Set<TenderCategoryCode>()
                .Include(c => c.CodeMaster)
                .Where(c => c.TenderId == tenderId)
                .ToListAsync();

            var categoryCodeStr = categoryCodes.Any()
                ? string.Join(" AND ", categoryCodes
                    .Where(c => c.CodeMaster != null)
                    .Select(c => c.CodeMaster!.Code))
                : null;

            // Validity: days between advertisement start and end
            string? validity = null;
            if (ad != null)
            {
                var days = (ad.AdvertisementEndDate - ad.AdvertisementStartDate).Days;
                validity = days > 0 ? $"{days} DAYS" : null;
            }

            // Closing time formatted as "hh:mm tt"
            var closingTime = ad != null
                ? DateTime.Today.Add(ad.EndTime).ToString("hh:mm tt")
                : null;

            // Vendors: query by matching category codes and join financial/certificate data
            var codeMasterIds = categoryCodes.Select(c => c.CodeMasterId).Distinct().ToList();
            var categoryIds   = categoryCodes.Where(c => c.CategoryId.HasValue)
                                             .Select(c => c.CategoryId!.Value).Distinct().ToList();

            var vendorCategories = await _context.Set<VendorCategory>()
                .Where(vc =>
                    codeMasterIds.Contains(vc.CodeMasterId) ||
                    (vc.CategoryId.HasValue && categoryIds.Contains(vc.CategoryId.Value)))
                .Select(vc => vc.VendorId)
                .Distinct()
                .ToListAsync();

            var vendors = await _context.Set<Vendor>()
                .Include(v => v.VendorFinancial)
                .Where(v => vendorCategories.Contains(v.Id))
                .ToListAsync();

            // Registration expiry: latest active certificate per vendor matching tender codes
            var vendorIds = vendors.Select(v => v.Id).ToList();
            var certificates = await _context.Set<VendorCategoryCertificate>()
                .Where(c => vendorIds.Contains(c.VendorId) && codeMasterIds.Contains(c.CodeMasterId))
                .GroupBy(c => c.VendorId)
                .Select(g => new { VendorId = g.Key, Expiry = g.Max(c => c.EndDate) })
                .ToDictionaryAsync(x => x.VendorId, x => x.Expiry);

            var vendorDtos = vendors.Select((v, idx) =>
            {
                var bumi = v.VendorFinancial?.BumiputeraEquityPercentage > 50
                    ? "Bumiputera"
                    : "Non Bumiputera";

                certificates.TryGetValue(v.Id, out var expiry);

                return new TenderOpeningVendorDto
                {
                    Bil = idx + 1,
                    VendorName = v.CompanyName,
                    BumiStatus = bumi,
                    RegistrationExpiry = expiry,
                    OfferedPrice = null   // populated when vendor bid entity is available
                };
            }).ToList();

            return new TenderOpeningPageDto
            {
                TenderId = tender.Id,
                ReferenceId = "T" + tender.Id.ToString("D7"),
                ProjectName = tender.ProjectName,
                Summary = new TenderOpeningSummaryRowDto
                {
                    ClosingDate = ad?.AdvertisementEndDate,
                    ClosingTime = closingTime,
                    Type = tender.TenderCategory?.Name,
                    CategoryCode = categoryCodeStr,
                    EstimationCost = tender.EstimatedPrices,
                    Validity = validity
                },
                Vendors = vendorDtos
            };
        }

        // ── Tender Award — List ────────────────────────────────────────────────────
        public async Task<List<TenderAwardListDto>> GetTenderAwardListAsync(string? referenceId, string? projectName)
        {
            var query = _context.TenderApplications.AsQueryable();

            if (!string.IsNullOrWhiteSpace(referenceId))
            {
                var refNum = referenceId.TrimStart('T', 't');
                if (int.TryParse(refNum, out var refId))
                    query = query.Where(t => t.Id == refId);
            }

            if (!string.IsNullOrWhiteSpace(projectName))
                query = query.Where(t => t.ProjectName != null && t.ProjectName.Contains(projectName));

            var tenders = await query
                .Include(t => t.TenderCategory)
                .ToListAsync();

            var tenderIds = tenders.Select(t => t.Id).ToList();

            var recommendations = await _context.TenderRecommendations
                .Where(r => tenderIds.Contains(r.TenderId))
                .Include(r => r.RecommendedVendor)
                .ToListAsync();

            var awards = await _context.TenderAwards
                .Where(a => tenderIds.Contains(a.TenderId))
                .ToListAsync();

            var recDict = recommendations
                .GroupBy(r => r.TenderId)
                .ToDictionary(g => g.Key, g => g.First());
            var awardDict = awards
                .GroupBy(a => a.TenderId)
                .ToDictionary(g => g.Key, g => g.First());

            return tenders.Select(t =>
            {
                recDict.TryGetValue(t.Id, out var rec);
                awardDict.TryGetValue(t.Id, out var award);
                return new TenderAwardListDto
                {
                    TenderId = t.Id,
                    ReferenceId = "T" + t.Id.ToString("D7"),
                    ProjectName = t.ProjectName,
                    RecommendedVendorName = rec?.RecommendedVendor?.CompanyName,
                    AwardStatus = award != null ? "Awarded" : "Pending"
                };
            }).ToList();
        }

        // ── Tender Award — Get Page ────────────────────────────────────────────────
        public async Task<TenderAwardPageDto?> GetTenderAwardPageAsync(int tenderId)
        {
            var tender = await _context.TenderApplications
                .Include(t => t.TenderCategory)
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            // Minutes of meeting
            var minutes = await _context.TenderAwardMinutesOfMeetings
                .Where(m => m.TenderId == tenderId)
                .OrderBy(m => m.MeetingDate)
                .Select(m => new TenderAwardMinutesDto
                {
                    Id = m.Id,
                    TenderId = m.TenderId,
                    MeetingDate = m.MeetingDate,
                    MeetingOutcome = m.MeetingOutcome,
                    AttachmentPath = m.AttachmentPath,
                    AttachmentFileName = m.AttachmentFileName
                })
                .ToListAsync();

            // Existing award record
            var award = await _context.TenderAwards
                .Include(a => a.AwardedVendor)
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            // Vendor options — only the recommended vendor from evaluation
            var recommendation = await _context.TenderRecommendations
                .Where(r => r.TenderId == tenderId)
                .Include(r => r.RecommendedVendor)
                .FirstOrDefaultAsync();

            var vendorOptions = new List<TenderAwardVendorOptionDto>();
            if (recommendation?.RecommendedVendor != null)
            {
                vendorOptions.Add(new TenderAwardVendorOptionDto
                {
                    VendorId = recommendation.RecommendedVendor.Id,
                    VendorName = recommendation.RecommendedVendor.CompanyName
                });
            }

            // Build appointment DTO
            var appointment = new TenderAwardVendorAppointmentDto();
            if (award != null)
            {
                appointment.AwardedVendorId = award.AwardedVendorId;
                appointment.AwardedVendorName = award.AwardedVendor?.CompanyName;
                appointment.ProjectValue = award.ProjectValue;
                appointment.YearlyExpenses = award.YearlyExpenses;
                appointment.ProjectStartDate = award.ProjectStartDate;
                appointment.ProjectEndDate = award.ProjectEndDate;
                appointment.Agreement = award.Agreement;
                appointment.AgreementDateSigned = award.AgreementDateSigned;
                appointment.PONumber = award.PONumber;
            }

            // Compute insurance from project value and dates
            var projectValue = award?.ProjectValue ?? tender.EstimatedPrices ?? 0m;
            var startDate = award?.ProjectStartDate;
            var endDate = award?.ProjectEndDate;

            var insurance = new TenderAwardInsuranceDto
            {
                PublicLiabilityValue = Math.Max(projectValue * 0.10m, 2_000_000m),
                PublicLiabilityPeriodStart = startDate,
                PublicLiabilityPeriodEnd = startDate?.AddYears(1),

                ContractorAtRiskValue = projectValue * 1.10m,
                ContractorAtRiskPeriodStart = startDate,
                ContractorAtRiskPeriodEnd = endDate,

                WorksmanCompensationValue = 2_000_000m,
                WorksmanCompensationPeriodStart = startDate,
                WorksmanCompensationPeriodEnd = startDate?.AddYears(1),

                LADValue = 500m
            };

            return new TenderAwardPageDto
            {
                TenderId = tender.Id,
                ReferenceId = "T" + tender.Id.ToString("D7"),
                ProjectName = tender.ProjectName,
                MinutesOfMeetings = minutes,
                VendorAppointment = appointment,
                Insurance = insurance,
                VendorOptions = vendorOptions
            };
        }

        // ── Tender Award — Save Award (Vendor Appointment) ────────────────────────
        public async Task SaveTenderAwardAsync(SaveTenderAwardDto dto)
        {
            var award = await _context.TenderAwards
                .FirstOrDefaultAsync(a => a.TenderId == dto.TenderId);

            if (award == null)
            {
                award = new TenderAward { TenderId = dto.TenderId };
                _context.TenderAwards.Add(award);
            }

            var ap = dto.VendorAppointment;
            award.AwardedVendorId = ap.AwardedVendorId;
            award.ProjectValue = ap.ProjectValue;
            award.YearlyExpenses = ap.YearlyExpenses;
            award.ProjectStartDate = ap.ProjectStartDate;
            award.ProjectEndDate = ap.ProjectEndDate;
            award.Agreement = ap.Agreement;
            award.AgreementDateSigned = ap.AgreementDateSigned;
            award.PONumber = ap.PONumber;

            await _context.SaveChangesAsync();
        }

        // ── Tender Award — Save Minutes of Meeting (Add/Edit popup) ───────────────
        public async Task<TenderAwardMinutesDto> SaveTenderAwardMinutesAsync(SaveTenderAwardMinutesDto dto)
        {
            TenderAwardMinutesOfMeeting entity;

            if (dto.Id > 0)
            {
                entity = await _context.TenderAwardMinutesOfMeetings
                    .FirstOrDefaultAsync(m => m.Id == dto.Id)
                    ?? throw new KeyNotFoundException($"Minutes record {dto.Id} not found");
            }
            else
            {
                entity = new TenderAwardMinutesOfMeeting { TenderId = dto.TenderId };
                _context.TenderAwardMinutesOfMeetings.Add(entity);
            }

            entity.MeetingDate = dto.MeetingDate;
            entity.MeetingOutcome = dto.MeetingOutcome;
            entity.AttachmentPath = dto.AttachmentPath;
            entity.AttachmentFileName = dto.AttachmentFileName;

            await _context.SaveChangesAsync();

            return new TenderAwardMinutesDto
            {
                Id = entity.Id,
                TenderId = entity.TenderId,
                MeetingDate = entity.MeetingDate,
                MeetingOutcome = entity.MeetingOutcome,
                AttachmentPath = entity.AttachmentPath,
                AttachmentFileName = entity.AttachmentFileName
            };
        }

        // ── Tender Award — Delete Minutes ─────────────────────────────────────────
        public async Task DeleteTenderAwardMinutesAsync(int minutesId)
        {
            var entity = await _context.TenderAwardMinutesOfMeetings
                .FirstOrDefaultAsync(m => m.Id == minutesId);

            if (entity == null) return;

            _context.TenderAwardMinutesOfMeetings.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // ── Vendor Performance — Get Page ─────────────────────────────────────────
        public async Task<VendorPerformancePageDto?> GetVendorPerformancePageAsync(int tenderId)
        {
            var tender = await _context.TenderApplications
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var award = await _context.TenderAwards
                .Include(a => a.AwardedVendor)
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            // Existing performance record (if already saved)
            var performance = await _context.VendorPerformances
                .Include(p => p.Scores)
                .Include(p => p.Feedbacks)
                .FirstOrDefaultAsync(p => p.TenderId == tenderId);

            // Build performance criteria rows (merge master list with saved ratings)
            var savedScores = performance?.Scores?.ToDictionary(s => s.Category)
                              ?? new Dictionary<string, VendorPerformanceScore>();

            var criteriaRows = VendorPerformanceCriteriaList.Items.Select(c =>
            {
                var saved = savedScores.GetValueOrDefault(c.Category);
                var rating = saved?.Rating ?? 0;
                return new VendorPerformanceCriteriaRowDto
                {
                    Category = c.Category,
                    Indicator = c.Indicator,
                    Weightage = c.Weightage,
                    Rating = rating,
                    Score = c.Weightage * rating / 5
                };
            }).ToList();

            int totalScore = criteriaRows.Sum(r => r.Score);

            // Build stakeholder feedback rows (merge fixed questions with saved feedback)
            var savedFeedbacks = performance?.Feedbacks?.ToDictionary(f => f.QuestionOrder)
                                 ?? new Dictionary<int, VendorPerformanceFeedback>();

            var feedbackRows = VendorPerformanceFeedbackList.Questions
                .Select((question, idx) =>
                {
                    var order = idx + 1;
                    var saved = savedFeedbacks.GetValueOrDefault(order);
                    var score = saved?.FeedbackScore ?? 0;
                    var label = score > 0 ? VendorPerformanceFeedbackList.ScaleLabels[score - 1] : string.Empty;
                    return new VendorPerformanceFeedbackRowDto
                    {
                        QuestionOrder = order,
                        Description = question,
                        FeedbackScore = score,
                        FeedbackLabel = label
                    };
                }).ToList();

            // Reviewer info
            var reviewer = new VendorPerformanceReviewerDto
            {
                PICName = performance?.PICName,
                Department = performance?.Department,
                Designation = performance?.Designation,
                MobileNo = performance?.MobileNo,
                CreatedDateTime = performance?.CreatedDate
            };

            return new VendorPerformancePageDto
            {
                TenderId = tenderId,
                VendorName = award?.AwardedVendor?.CompanyName,
                ReferenceId = "T" + tenderId.ToString("D7"),
                ProjectName = tender.ProjectName,
                AwardDate = award?.AgreementDateSigned ?? award?.CreatedDate,
                ReviewMonthYear = performance?.ReviewMonthYear,
                PerformanceScores = criteriaRows,
                TotalScore = totalScore,
                StakeholderFeedbacks = feedbackRows,
                Reviewer = reviewer
            };
        }

        // ── Vendor Performance — Save ─────────────────────────────────────────────
        public async Task SaveVendorPerformanceAsync(SaveVendorPerformanceDto dto, int userId)
        {
            // Load the current user for reviewer info
            var user = await _context.Users
                .Include(u => u.SiteLevel)
                .Include(u => u.Designation)
                .FirstOrDefaultAsync(u => u.Id == userId);

            // Upsert VendorPerformance
            var performance = await _context.VendorPerformances
                .Include(p => p.Scores)
                .Include(p => p.Feedbacks)
                .FirstOrDefaultAsync(p => p.TenderId == dto.TenderId);

            if (performance == null)
            {
                performance = new VendorPerformance
                {
                    TenderId = dto.TenderId,
                    PICName = user?.FullName,
                    Department = user?.SiteLevel?.Name,
                    Designation = user?.Designation?.Name,
                    MobileNo = user?.MobileNo
                };
                _context.VendorPerformances.Add(performance);
                await _context.SaveChangesAsync();
            }

            performance.ReviewMonthYear = dto.ReviewMonthYear;

            // Sync scores — remove old, add new
            if (performance.Scores != null)
                _context.VendorPerformanceScores.RemoveRange(performance.Scores);

            var criteriaLookup = VendorPerformanceCriteriaList.Items
                .ToDictionary(c => c.Category);

            var newScores = dto.Scores.Select(s =>
            {
                criteriaLookup.TryGetValue(s.Category, out var def);
                var weightage = def?.Weightage ?? 0;
                return new VendorPerformanceScore
                {
                    VendorPerformanceId = performance.Id,
                    Category = s.Category,
                    Rating = s.Rating,
                    Score = weightage * s.Rating / 5
                };
            }).ToList();

            performance.TotalScore = newScores.Sum(s => s.Score);
            _context.VendorPerformanceScores.AddRange(newScores);

            // Sync feedbacks — remove old, add new
            if (performance.Feedbacks != null)
                _context.VendorPerformanceFeedbacks.RemoveRange(performance.Feedbacks);

            var newFeedbacks = dto.Feedbacks.Select(f => new VendorPerformanceFeedback
            {
                VendorPerformanceId = performance.Id,
                QuestionOrder = f.QuestionOrder,
                FeedbackScore = f.FeedbackScore
            }).ToList();

            _context.VendorPerformanceFeedbacks.AddRange(newFeedbacks);

            await _context.SaveChangesAsync();
        }
    }


}
