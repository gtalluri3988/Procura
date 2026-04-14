using Azure.Core;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Repositories.Interfaces;
using Procura.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IMasterDataRepository _masterDataRepository;

        private const int MaxCategoriesAllowed = 2;
        private const int MaxSubCategoriesPerCategory = 3;
        private const int MaxChangesAllowed = 2;

        public VendorService(IVendorRepository vendorRepository, IMasterDataRepository masterDataRepository)
        {
            _vendorRepository = vendorRepository;
            _masterDataRepository = masterDataRepository;
        }

        //public async Task SaveStepAsync(int vendorId, VendorRegistrationStep Step, StepSaveRequest request)
        //{
        //    switch (Step)
        //    {
        //        case VendorRegistrationStep.Profile:
        //            await _vendorRepository.UpdateVendorAsync(_mapper.Map<Vendor>(request.Profile));
        //            break;

        //        case VendorRegistrationStep.Members:
        //            await _vendorRepository.SaveMembersAsync(
        //                vendorId,
        //                _mapper.Map<List<VendorMember>>(request.Members));
        //            break;

        //        case VendorRegistrationStep.Financial:
        //            await _vendorRepository.SaveFinancialAsync(
        //                vendorId,
        //                _mapper.Map<VendorFinancial>(request.Financial));
        //            break;

        //        case VendorRegistrationStep.Category:
        //            await _vendorRepository.SaveVendorCategoriesAsync(
        //                vendorId,
        //                _mapper.Map<List<VendorCategory>>(request.Categories));
        //            break;

        //        case VendorRegistrationStep.Experience:
        //            await _vendorRepository.SaveExperiencesAsync(
        //                vendorId,
        //                request.Experiences);
        //            break;

        //        case VendorRegistrationStep.Declaration:
        //            await _vendorRepository.SaveDeclarationAsync(
        //                vendorId,
        //                _mapper.Map<VendorDeclaration>(request.Declaration));
        //            break;

        //        default:
        //            throw new Exception("Invalid step.");
        //    }
        //}



        // NEW: Accept raw JSON and dispatch to existing SaveStepAsync by deserializing only what's needed
        public async Task SaveStepRawAsync(int vendorId, VendorRegistrationStep step, JsonElement data)
        {
            // Build a minimal StepSaveRequest with only the relevant property populated
            var request = new StepSaveRequest { Step = step };

            switch (step)
            {
                case VendorRegistrationStep.Profile:
                    request.Profile = JsonSerializer.Deserialize<VendorProfileDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                case VendorRegistrationStep.Members:
                    // members wrapper DTO expected by existing code
                    request.Members = JsonSerializer.Deserialize<VendorMemberDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                case VendorRegistrationStep.Financial:
                    request.Financial = JsonSerializer.Deserialize<VendorFinancialDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                case VendorRegistrationStep.Category:
                    request.Categories = JsonSerializer.Deserialize<List<VendorCategoryDto>>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                case VendorRegistrationStep.Experience:
                    request.Experiences = JsonSerializer.Deserialize<List<VendorExperienceDto>>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                case VendorRegistrationStep.Declaration:
                    request.Declaration = JsonSerializer.Deserialize<VendorDeclarationDto>(data.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    break;

                default:
                    throw new ArgumentException("Invalid step", nameof(step));
            }

            // Reuse the existing typed SaveStepAsync implementation
            await SaveStepAsync(vendorId, step, request);
        }
        public async Task SaveStepAsync(int vendorId, VendorRegistrationStep step, StepSaveRequest request)
        {
            switch (step)
            {
                case VendorRegistrationStep.Profile:
                    {
                        var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
                        if (vendor == null)
                            throw new Exception("Vendor not found");

                        // Manual mapping
                        vendor.CompanyName = request.Profile.CompanyName;
                        vendor.Address = request.Profile.Address;
                        vendor.City = request.Profile.City;
                        vendor.StateId = request.Profile.StateId;
                        vendor.CountryId = request.Profile.CountryId;
                        vendor.Email = request.Profile.Email;
                        vendor.Website = request.Profile.Website;
                        vendor.PicName = request.Profile.PicName;
                        vendor.PicMobileNo = request.Profile.PicMobileNo;
                        await _vendorRepository.UpdateVendorAsync(vendor);
                        break;
                    }

                case VendorRegistrationStep.Members:
                    {
                        var members = request?.Members
                            ?? throw new Exception("Members section is required.");

                        var shareholders = members.Shareholders ?? new List<VendorShareholder>();
                        var directors = members.Directors ?? new List<VendorDirector>();
                        var manpower = members.VendorManpower ?? new VendorManpower();
                        var staff = members.StaffDeclarations ?? new List<VendorStaffDeclaration>();

                        await _vendorRepository.SaveMembersAsync(
                            vendorId,
                            shareholders,
                            directors,
                            manpower,
                            staff);

                        break;
                    }

                case VendorRegistrationStep.Financial:
                    {
                        var dto = request.Financial;

                        var financial = new VendorFinancial
                        {
                            VendorId = vendorId,
                            PaidUpCapital = dto.PaidUpCapital,
                            AuthorizedCapital = dto.AuthorizedCapital,
                            WorkingCapital = dto.WorkingCapital,
                            LiquidCapital = dto.LiquidCapital,
                            AssetBalance = dto.AssetBalance,
                            BumiputeraEquityAmount = dto.BumiputeraEquityAmount,
                            BumiputeraEquityPercentage = dto.BumiputeraEquityPercentage,
                            NonBumiputeraEquityAmount = dto.NonBumiputeraEquityAmount,
                            NonBumiputeraEquityPercentage = dto.NonBumiputeraEquityPercentage,
                            RollingCapital = dto.RollingCapital,
                            TotalOverdraft = dto.TotalOverdraft,
                            OthersCredit = dto.OthersCredit
                        };

                        await _vendorRepository.SaveFinancialAsync(vendorId, financial);
                        break;
                    }

                case VendorRegistrationStep.Category:
                    {
                        var categories = new List<VendorCategory>();
                        var categoriesCertificates = new List<VendorCategoryCertificate>();

                        if (request.Categories != null && request.Categories.Count > 0)
                        {
                            foreach (var dto in request.Categories)
                            {
                                categories.Add(new VendorCategory
                                {
                                    VendorId = vendorId,
                                    CodeMasterId = dto.CodeMasterId,
                                    CategoryId = dto.CategoryId,
                                    SubCategoryId = dto.SubCategoryId,
                                    ActivityId = dto.ActivityId,
                                    //CertificatePath = dto.CertificatePath,
                                    //StartDate = dto.StartDate,
                                    //EndDate = dto.EndDate
                                });
                            }
                        }
                        if (request.VendorCategoryCertificate != null && request.VendorCategoryCertificate.Count > 0)
                        {
                            foreach (var dto in request.VendorCategoryCertificate)
                            {
                                categoriesCertificates.Add(new VendorCategoryCertificate
                                {
                                    VendorId = vendorId,
                                    CodeMasterId = dto.CodeMasterId,
                                    CertificatePath = dto.CertificatePath,
                                    StartDate = dto.StartDate,
                                    EndDate = dto.EndDate
                                });
                            }
                        }

                        await _vendorRepository.SaveCategoriesAsync(vendorId, categories, categoriesCertificates);
                        break;
                    }

                case VendorRegistrationStep.Experience:
                    {
                        var experiences = new List<VendorExperience>();

                        foreach (var dto in request.Experiences)
                        {
                            experiences.Add(new VendorExperience
                            {
                                VendorId = vendorId,
                                ProjectName = dto.ProjectName,
                                Organization = dto.Organization,
                                ProjectValue = dto.ProjectValue,
                                Status = dto.Status,
                                CompletionYear = dto.CompletionYear,
                                AttachmentPath = dto.AttachmentPath
                            });
                        }

                        await _vendorRepository.SaveExperiencesAsync(vendorId, experiences);
                        break;
                    }

                case VendorRegistrationStep.Declaration:
                    {
                        var dto = request.Declaration;

                        var declaration = new VendorDeclaration
                        {
                            VendorId = vendorId,
                            EsqQuestionnaireAccepted = dto.EsqQuestionnaireAccepted,
                            ConfidentialityAgreementAccepted = dto.ConfidentialityAgreementAccepted,
                            PoTermsAccepted = dto.PoTermsAccepted,
                            CodeOfConductAccepted = dto.CodeOfConductAccepted,
                            PdpAAccepted = dto.PdpAAccepted,
                            EnvironmentalPolicyAccepted = dto.EnvironmentalPolicyAccepted,
                            NoGiftPolicyAccepted = dto.NoGiftPolicyAccepted,
                            IntegrityDeclarationAccepted = dto.IntegrityDeclarationAccepted,
                            FinalDeclarationAccepted = dto.FinalDeclarationAccepted
                        };

                        await _vendorRepository.SaveDeclarationAsync(vendorId, declaration);
                        break;
                    }

                default:
                    throw new Exception("Invalid step.");
            }
        }

        public async Task SaveSSMResponse(string input, string response)
        {
            await _vendorRepository.SaveSSMResponse(input,response);
        }

        public async Task<VendorProfileDto> RegisterVendor(CreateVendorRequest request)
        {
            var vendor = new Vendor
            {
                CompanyEntityTypeId = request.CompanyEntityTypeId,
                RocNumber = request.RocNumber,
                PasswordHash = request.PasswordHash,
                RoleId = (int)Roles.Vendor,
                
            };

            // repository returns the persisted profile DTO (including CurrentStep)
            var profile = await _vendorRepository.RegisterVendor(vendor);

            // compute and attach NextStep for the frontend
            profile.NextStep = ComputeNextStep(profile.CurrentStep);

            return profile;
        }

        private VendorRegistrationStep? ComputeNextStep(VendorRegistrationStep? current)
        {
            if (current == null) return VendorRegistrationStep.Profile;

            var values = Enum.GetValues(typeof(VendorRegistrationStep)).Cast<VendorRegistrationStep>().ToArray();
            var idx = Array.IndexOf(values, current.Value);
            if (idx >= 0 && idx < values.Length - 1)
                return values[idx + 1];

            return null; // no next step (already last)
        }

        public async Task<VendorRegistrationStep?> SaveProfileAsync(int vendorId, VendorProfileDto request)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);
            if (vendor == null)
                throw new Exception("Vendor not found");

            // Manual mapping
            vendor.CompanyName = request.CompanyName;
            vendor.Address = request.Address;
            vendor.City = request.City;
            vendor.StateId = request.StateId;
            vendor.CountryId = request.CountryId;
            vendor.Email = request.Email;
            vendor.Website = request.Website;
            vendor.PicName = request.PicName;
            vendor.PicMobileNo = request.PicMobileNo;
            vendor.Postcode= request.Postcode;
            vendor.OfficePhoneNo = request.OfficePhoneNo;
            vendor.FaxNo = request.FaxNo;
            vendor.IndustryTypeId = request.IndustryTypeId;
            vendor.PicEmail = request.PicEmail;
            vendor.BusinessCoverageArea = request.BusinessCoverageArea;
            vendor.DateOfIncorporation= request.DateOfIncorporation;
            vendor.Form24AttachmentPath = request.Form24AttachmentPath;
            vendor.FileName = request.FileName;
            vendor.IsRegistrationComplete = request.IsRegistrationComplete;
            return await _vendorRepository.UpdateVendorAsync(vendor);

        }

        public async Task<VendorRegistrationStep?> SaveMembersAsync(int vendorId, VendorMemberDto request)
        {
            var members = request
                           ?? throw new Exception("Members section is required.");

            var shareholders = members.Shareholders ?? new List<VendorShareholder>();
            var directors = members.Directors ?? new List<VendorDirector>();
            var manpower = members.VendorManpower ?? new VendorManpower();
            var staff = members.StaffDeclarations ?? new List<VendorStaffDeclaration>();

            return await _vendorRepository.SaveMembersAsync(
                vendorId,
                shareholders,
                directors,
                manpower,
                staff);
        }

        public async Task<VendorRegistrationStep?> SaveFinancialAsync(int vendorId, VendorFinancialDto request)
        {
            var dto = request;

            var financial = new VendorFinancial
            {
                VendorId = vendorId,
                Id=request.Id,
                PaidUpCapital = dto.PaidUpCapital,
                AuthorizedCapital = dto.AuthorizedCapital,
                WorkingCapital = dto.WorkingCapital,
                LiquidCapital = dto.LiquidCapital,
                AssetBalance = dto.AssetBalance,
                BumiputeraEquityAmount = dto.BumiputeraEquityAmount,
                BumiputeraEquityPercentage = dto.BumiputeraEquityPercentage,
                NonBumiputeraEquityAmount = dto.NonBumiputeraEquityAmount,
                NonBumiputeraEquityPercentage = dto.NonBumiputeraEquityPercentage,
                RollingCapital = dto.RollingCapital,
                TotalOverdraft = dto.TotalOverdraft,
                OthersCredit = dto.OthersCredit,
                CreditFacilities=dto.CreditFacilities,
                LatestBankStatementPath= dto.LatestBankStatementPath,
                Tax =dto.Tax,
                Bank=dto.Bank
                
            };

            return await _vendorRepository.SaveFinancialAsync(vendorId, financial);
        }

        public async Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, VendorCategoryRequest request)
        {
            var categories = new List<VendorCategory>();
            var categoriesCertificate = new List<VendorCategoryCertificate>();

            if (request.VendorCategoryDto != null && request.VendorCategoryDto.Count>0)
            {
                foreach (var dto in request.VendorCategoryDto)
                {
                    categories.Add(new VendorCategory
                    {
                        VendorId = vendorId,
                        CodeMasterId = dto.CodeMasterId,
                        CategoryId = dto.CategoryId,
                        SubCategoryId = dto.SubCategoryId,
                        ActivityId = dto.ActivityId,

                    });
                }
            }

            if (request.VendorCategoryCertificateDto != null && request.VendorCategoryCertificateDto.Count > 0)
            {
                foreach (var dto in request.VendorCategoryCertificateDto)
                {
                    categoriesCertificate.Add(new VendorCategoryCertificate
                    {
                        VendorId = vendorId,
                        CodeMasterId = dto.CodeMasterId,
                        CertificatePath = dto.CertificatePath,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    });
                }
            }

            return await _vendorRepository.SaveCategoriesAsync(vendorId, categories, categoriesCertificate);
        }

        public async Task<VendorRegistrationStep?> SaveExperiencesAsync(int vendorId, List<VendorExperienceDto> request)
        {
            var experiences = new List<VendorExperience>();

            foreach (var dto in request)
            {
                experiences.Add(new VendorExperience
                {
                    VendorId = vendorId,
                    ProjectName = dto.ProjectName,
                    Organization = dto.Organization,
                    ProjectValue = dto.ProjectValue,
                    Status = dto.Status,
                    CompletionYear = dto.CompletionYear,
                    AttachmentPath = dto.AttachmentPath
                });
            }

            return await _vendorRepository.SaveExperiencesAsync(vendorId, experiences);
        }

        public async Task<VendorRegistrationStep?> SaveDeclarationAsync(int vendorId, VendorDeclarationDto request)
        {
            var dto = request;

            var declaration = new VendorDeclaration
            {
                VendorId = vendorId,
                EsqQuestionnaireAccepted = dto.EsqQuestionnaireAccepted,
                ConfidentialityAgreementAccepted = dto.ConfidentialityAgreementAccepted,
                PoTermsAccepted = dto.PoTermsAccepted,
                CodeOfConductAccepted = dto.CodeOfConductAccepted,
                PdpAAccepted = dto.PdpAAccepted,
                EnvironmentalPolicyAccepted = dto.EnvironmentalPolicyAccepted,
                NoGiftPolicyAccepted = dto.NoGiftPolicyAccepted,
                IntegrityDeclarationAccepted = dto.IntegrityDeclarationAccepted,
                FinalDeclarationAccepted = dto.FinalDeclarationAccepted
            };

            return await _vendorRepository.SaveDeclarationAsync(vendorId, declaration);
        }

        public async Task<PaymentDetailsDTO> GetPaymentDetailsAsync(int vendorId)
        {
            return await _vendorRepository.GetPaymentDetailsAsync(vendorId);
        }

        public async Task<IEnumerable<CompanyCategoryDto>> GetCompanyTypes()
        {
            return await _vendorRepository.GetCompanyTypes();
        }

        public async Task<IEnumerable<CompanyCategoryDto>> GetCompanyEntitiesByTypeIdAsync(int TypeId)
        {
            return await _vendorRepository.GetCompanyEntitiesByTypeIdAsync(TypeId);
        }

        public async Task<IEnumerable<VendorProfileDto>> GetVendorListAsync()
        {
            return await _vendorRepository.GetVendorListAsync();
        }

        public async Task<VendorDashboardDto> GetVendorDashboardAsync()
        {
            return await _vendorRepository.GetVendorDashboardAsync();
        }

        public async Task<IEnumerable<VendorProfileDto>> GetVendorListAsync(VendorSearchRequest? request)
        {
            return await _vendorRepository.GetVendorListAsync(request);
        }

        public async Task<IEnumerable<IndustryTypeDto>> BindIndustryTypeListAsync()
        {
            return await _vendorRepository.BindIndustryTypeListAsync();
        }

        public async Task<Vendor?> GetVendorFullDetailsAsync(int vendorId)
        {
            return await _vendorRepository.GetVendorFullDetailsAsync(vendorId);
        }

        public async Task SaveQuestionAnswers(int vendorId, List<QuestionAnswerDto> answers)
        {
            await _vendorRepository.SaveQuestionAnswers(vendorId, answers);
        }

        public async Task<List<QuestionAnswerDto>> GetQuestionAnswersByQuestionnaireId(int questionnaireId, int vendorId)
        {
             return await _vendorRepository.GetQuestionAnswersByQuestionnaireId(questionnaireId, vendorId);
        }

        public async Task<CategoryChangeValidationResult> ValidateCategoryChangeAsync(int vendorId)
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

            // Rule 3: Freeze period — no changes allowed for N months after approval
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

            // Rule 4: Max 2 changes within the N-year validity period
            if (vendor.ApprovalDatetime.HasValue && yearsSetting > 0)
            {
                var validityStart = vendor.ApprovalDatetime.Value;
                var validityEnd = validityStart.AddYears(yearsSetting);
                result.ValidityEndDate = validityEnd;

                var changeCount = await _vendorRepository.GetCategoryChangeCountAsync(
                    vendorId, validityStart, validityEnd);

                result.RemainingChanges = Math.Max(0, MaxChangesAllowed - changeCount);

                if (changeCount >= MaxChangesAllowed)
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

            // Rule 1: Current category count (informational for UI)
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
                // Vendor not yet approved — initial registration, no freeze/change limits apply
                result.RemainingChanges = MaxChangesAllowed;
            }

            return result;
        }
    }
}
