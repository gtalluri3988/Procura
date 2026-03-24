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
    }


}
