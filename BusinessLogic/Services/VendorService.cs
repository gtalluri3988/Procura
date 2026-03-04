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

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
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

                        foreach (var dto in request.Categories)
                        {
                            categories.Add(new VendorCategory
                            {
                                VendorId = vendorId,
                                MasterCategoryId = dto.MasterCategoryId,
                                CertificatePath = dto.CertificatePath,
                                StartDate = dto.StartDate,
                                EndDate = dto.EndDate
                            });
                        }

                        await _vendorRepository.SaveCategoriesAsync(vendorId, categories);
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
                RoleId = (int)Roles.Vendor
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
            vendor.Form24AttachmentPath = request.Form24AttachmentPath;
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

            return await _vendorRepository.SaveFinancialAsync(vendorId, financial);
        }

        public async Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, List<VendorCategoryDto> request)
        {
            var categories = new List<VendorCategory>();

            foreach (var dto in request)
            {
                categories.Add(new VendorCategory
                {
                    VendorId = vendorId,
                    MasterCategoryId = dto.MasterCategoryId,
                    CertificatePath = dto.CertificatePath,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                });
            }

            return await _vendorRepository.SaveCategoriesAsync(vendorId, categories);
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
    }
}
