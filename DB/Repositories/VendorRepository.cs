using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Model;
using DB.Profiles;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DB.Repositories
{
    public class VendorRepository : RepositoryBase<Vendor, VendorProfileDto>, IVendorRepository
    {

        private readonly IConfiguration _configuration;
        public VendorRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(context, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Unconditionally set current completed step for the vendor.
        /// Use only when you want to force-set the step.
        /// </summary>
        public async Task UpdateStepAsync(int vendorId, VendorRegistrationStep step)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);

            if (vendor == null)
                throw new Exception("Vendor not found");

            vendor.CurrentStep = step;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Move to next step only when nextStep == current + 1.
        /// Kept for strict flow enforcement if you need it.
        /// </summary>
        public async Task MoveToNextStepAsync(int vendorId, VendorRegistrationStep nextStep)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);

            if (vendor == null)
                throw new Exception("Vendor not found");

            // Prevent skipping steps
            if ((int)nextStep != (int)vendor.CurrentStep + 1)
                throw new Exception("Invalid step flow.");

            vendor.CurrentStep = nextStep;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Advance the vendor's completed step to targetStep if it is a forward move.
        /// This is idempotent: repeated saves for the same step will not error.
        /// </summary>
        private async Task AdvanceStepIfNeededAsync(int vendorId, VendorRegistrationStep targetStep)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null)
                throw new Exception("Vendor not found");

            if ((int)vendor.CurrentStep < (int)targetStep)
            {
                vendor.CurrentStep = targetStep;
                await _context.SaveChangesAsync();
            }
        }

        #region CREATE ACCOUNT

        public async Task<int> CreateVendorAsync(Vendor vendor)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                vendor.CreatedDate = DateTime.UtcNow;

                vendor.VendorCodeStatus = VendorStatus.Draft.GetDisplayName();
                vendor.CurrentStep = VendorRegistrationStep.CreateAccount;

                await _context.Vendors.AddAsync(vendor);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return vendor.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region UPDATE PROFILE

        /// <summary>
        /// Update vendor's scalar/profile fields without blindly replacing navigation collections.
        /// Returns next step after completing Profile.
        /// </summary>
        public async Task<VendorRegistrationStep?> UpdateVendorAsync(Vendor vendor)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(vendor.Form24AttachmentPath))
                {
                    //var uploadPath = _configuration["FileSettings:UploadPath"];
                    //string filepath = Path.Combine(uploadPath, $"vendor_{vendor.Id}_form24.pdf");
                    //if (File.Exists(filepath))
                    //{
                    //    File.Delete(filepath);
                    //}
                    //SaveBase64ToFile(vendor.Form24AttachmentPath, filepath);
                    //vendor.Form24AttachmentPath = filepath;





                    var uploadRoot = _configuration["FileSettings:UploadPath"];
                    if (string.IsNullOrWhiteSpace(uploadRoot))
                    {
                        uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "VendorUploads");
                    }
                    var certFolder = Path.Combine(uploadRoot, "Vendor");
                    if (!Directory.Exists(certFolder))
                        Directory.CreateDirectory(certFolder);



                    // Heuristic: if contains ',' (data URI) or is long (likely base64), attempt to save.
                    if (vendor.Form24AttachmentPath.Contains(",") || vendor.Form24AttachmentPath.Length > 200)
                    {
                        var safeFileName = $"vendor_{vendor.Id}_vendor_{Guid.NewGuid():N}.pdf";
                        var fullPath = Path.Combine(certFolder, safeFileName);

                        // overwrite if same file exists is unlikely due to GUID, but remove if present
                        if (File.Exists(fullPath))
                            File.Delete(fullPath);

                        SaveBase64ToFile(vendor.Form24AttachmentPath, fullPath);

                        // save relative path for DB
                        vendor.Form24AttachmentPath = Path.GetRelativePath(uploadRoot, fullPath).Replace("\\", "/");
                    }














                }
                var existing = await _context.Vendors.FindAsync(vendor.Id);
                if (existing == null) throw new Exception("Vendor not found");

                // Update scalar properties only to avoid accidental navigation replacement
                _context.Entry(existing).CurrentValues.SetValues(new
                {
                    vendor.CompanyName,
                    vendor.DateOfIncorporation,
                    vendor.Address,
                    vendor.Postcode,
                    vendor.State,
                    vendor.City,
                    vendor.CountryId,
                    vendor.OfficePhoneNo,
                    vendor.FaxNo,
                    vendor.Email,
                    vendor.Website,
                    vendor.IndustryType,
                    vendor.BusinessCoverageArea,
                    vendor.PicName,
                    vendor.PicMobileNo,
                    vendor.PicEmail,
                    vendor.Form24AttachmentPath,
                    vendor.Status,
                    vendor.CreatedDate,
                    vendor.PasswordHash,
                    vendor.RocNumber,
                    vendor.CompanyEntityType,
                    vendor.IsRegistrationComplete,
                    vendor.FileName
                });

                await _context.SaveChangesAsync();

                // mark Profile step completed
                await AdvanceStepIfNeededAsync(vendor.Id, VendorRegistrationStep.Profile);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Profile);
        }

        #endregion
        public void SaveBase64ToFile(string base64String, string filePath)
        {
            // Remove data:image/png;base64, if exists
            if (base64String.Contains(","))
                base64String = base64String.Substring(base64String.IndexOf(",") + 1);

            byte[] fileBytes = Convert.FromBase64String(base64String);

            File.WriteAllBytes(filePath, fileBytes);
        }
        #region DELETE

        public async Task DeleteVendorAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null) return;

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteVendorAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null) return false;

            vendor.IsActive = false;

            // Best-effort cascade: if a User row exists for this vendor (RoleId 4 = Vendor,
            // matched by ROC number as username), deactivate it so User-level auth is also blocked.
            const int vendorRoleId = 4;
            var linkedUser = await _context.Users
                .FirstOrDefaultAsync(u => u.RoleId == vendorRoleId && u.UserName == vendor.RocNumber);
            if (linkedUser != null)
            {
                linkedUser.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region GET FULL DETAILS

        public async Task<Vendor?> GetVendorFullDetailsAsync(int vendorId)
        {
            return await _context.Vendors
                .Include(v => v.Shareholders)
                .Include(v => v.Directors)
                .Include(v => v.StaffDeclarations)
                .Include(v => v.VendorFinancial)
                .ThenInclude(v => v.Bank)
                .Include(v => v.VendorFinancial)
                .ThenInclude(v => v.Tax)
                .Include(v => v.VendorFinancial)
                .ThenInclude(v => v.CreditFacilities)
                .Include(v => v.VendorCategories)
                .ThenInclude(vc => vc.Category)
                .Include(v => v.VendorCategories)
                .ThenInclude(vc => vc.SubCategory)
                .Include(v => v.VendorCategories)
                .ThenInclude(vc => vc.Activity)
                .Include(v => v.VendorCategoryCertificates)
                .Include(v => v.VendorExperiences)
                .Include(v => v.VendorDeclaration)
                .Include(v => v.VendorPayment)
                .Include(v => v.VendorManpower)
                .FirstOrDefaultAsync(v => v.Id == vendorId);
        }

        public async Task<Vendor?> GetVendorByIdAsync(int vendorId)
        {
            return await _context.Vendors.FindAsync(vendorId);
        }

        #endregion

        #region SAVE MEMBERS

        /// <summary>
        /// Upsert shareholders, directors and staff declarations; update or insert manpower.
        /// Removes database items that are not present in incoming lists (synchronise).
        /// Returns the next logical step (Members -> next).
        /// </summary>
        public async Task<VendorRegistrationStep?> SaveMembersAsync(
    int vendorId,
    List<VendorShareholder> shareholders,
    List<VendorDirector> directors,
    VendorManpower manpower,
    List<VendorStaffDeclaration> staffDeclarations)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // -------------------------
                // 1️⃣ SHAREHOLDERS (sync)
                // -------------------------
                var existingShareholders = await _context.VendorShareholder
                    .Where(x => x.VendorId == vendorId)
                    .ToListAsync();

                // update or insert incoming
                foreach (var incoming in shareholders)
                {
                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existingShareholders.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                        {
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        }
                        else
                        {
                            // Id provided but not found -> treat as new
                            await _context.VendorShareholder.AddAsync(incoming);
                        }
                    }
                    else
                    {
                        await _context.VendorShareholder.AddAsync(incoming);
                    }
                }

                // remove those not present in incoming (by Id)
                var incomingShareholderIds = shareholders.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();
                var toRemoveShareholders = existingShareholders.Where(e => !incomingShareholderIds.Contains(e.Id)).ToList();
                if (toRemoveShareholders.Any())
                    _context.VendorShareholder.RemoveRange(toRemoveShareholders);


                // -------------------------
                // 2️⃣ DIRECTORS (sync)
                // -------------------------
                var existingDirectors = await _context.VendorDirectors
                    .Where(x => x.VendorId == vendorId)
                    .ToListAsync();

                foreach (var incoming in directors)
                {
                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existingDirectors.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        else
                            await _context.VendorDirectors.AddAsync(incoming);
                    }
                    else
                    {
                        await _context.VendorDirectors.AddAsync(incoming);
                    }
                }

                var incomingDirectorIds = directors.Where(d => d.Id > 0).Select(d => d.Id).ToHashSet();
                var toRemoveDirectors = existingDirectors.Where(e => !incomingDirectorIds.Contains(e.Id)).ToList();
                if (toRemoveDirectors.Any())
                    _context.VendorDirectors.RemoveRange(toRemoveDirectors);


                // -------------------------
                // 3️⃣ STAFF DECLARATIONS (sync)
                // -------------------------
                var existingStaff = await _context.VendorStaffDeclaration
                    .Where(x => x.VendorId == vendorId)
                    .ToListAsync();

                foreach (var incoming in staffDeclarations)
                {
                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existingStaff.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        else
                            await _context.VendorStaffDeclaration.AddAsync(incoming);
                    }
                    else
                    {
                        await _context.VendorStaffDeclaration.AddAsync(incoming);
                    }
                }

                var incomingStaffIds = staffDeclarations.Where(s => s.Id > 0).Select(s => s.Id).ToHashSet();
                var toRemoveStaff = existingStaff.Where(e => !incomingStaffIds.Contains(e.Id)).ToList();
                if (toRemoveStaff.Any())
                    _context.VendorStaffDeclaration.RemoveRange(toRemoveStaff);


                // -------------------------
                // 4️⃣ MANPOWER (One-to-One upsert)
                // -------------------------
                //var existingManpower = await _context.VendorManpowers
                //    .FirstOrDefaultAsync(x => x.VendorId == vendorId);

                //manpower.VendorId = vendorId;

                //if (existingManpower == null)
                //{
                //    await _context.VendorManpowers.AddAsync(manpower);
                //}
                //else
                //{
                //    _context.Entry(existingManpower).CurrentValues.SetValues(manpower);
                //}

                var existingManpower = await _context.VendorManpowers
    .FirstOrDefaultAsync(x => x.VendorId == vendorId);

                if (existingManpower == null)
                {
                    manpower.VendorId = vendorId;
                    await _context.VendorManpowers.AddAsync(manpower);
                }
                else
                {
                    existingManpower.NoOfBumiputera = manpower.NoOfBumiputera;
                    existingManpower.NoOfNonBumiputera = manpower.NoOfNonBumiputera;
                    existingManpower.BumiputeraPercentage = manpower.BumiputeraPercentage;
                    existingManpower.NonBumiputeraPercentage = manpower.NonBumiputeraPercentage;
                    existingManpower.TotalManpower = manpower.TotalManpower;
                    existingManpower.UpdatedBy = manpower.UpdatedBy;
                    existingManpower.UpdatedDate = DateTime.Now;
                }


                // -------------------------
                // SAVE ALL
                // -------------------------
                await _context.SaveChangesAsync();

                // Advance step
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Members);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Members);
        }


        #endregion

        #region SAVE FINANCIAL

        //public async Task<VendorRegistrationStep?> SaveFinancialAsync(int vendorId, VendorFinancial financial)
        //{
        //    using var tx = await _context.Database.BeginTransactionAsync();
        //    try
        //    {

        //        if (!string.IsNullOrEmpty(financial.LatestBankStatementPath))
        //        {
        //            var uploadPath = _configuration["FileSettings:UploadPath"];
        //            string filepath = Path.Combine(uploadPath, $"vendor_{vendorId}_LatestBankStatement.pdf");
        //            if (File.Exists(filepath))
        //            {
        //                File.Delete(filepath);
        //            }
        //            SaveBase64ToFile(financial.LatestBankStatementPath, filepath);
        //            financial.LatestBankStatementPath = filepath;
        //        }


        //        financial.VendorId = vendorId;

        //        var existing = await _context.VendorFinancials
        //            .FirstOrDefaultAsync(x => x.VendorId == vendorId);

        //        if (existing == null)
        //            await _context.VendorFinancials.AddAsync(financial);
        //        else
        //            _context.Entry(existing).CurrentValues.SetValues(financial);

        //        await _context.SaveChangesAsync();

        //        // mark Financial step completed
        //        await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Financial);

        //        await tx.CommitAsync();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        throw;
        //    }
        //    return ComputeNextStep(VendorRegistrationStep.Financial);
        //}


        public async Task<VendorRegistrationStep?> SaveFinancialAsync(int vendorId, VendorFinancial financial)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {

                if (!string.IsNullOrEmpty(financial.LatestBankStatementPath))
                {
                    // Heuristic: if contains ',' (data URI) or is long (likely base64), attempt to save.
                    if (financial.LatestBankStatementPath.Contains(",") || financial.LatestBankStatementPath.Length > 200)
                    {
                        var uploadPath = _configuration["FileSettings:UploadPath"];
                        string filepath = Path.Combine(uploadPath, $"vendor_{vendorId}_LatestBankStatement.pdf");
                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                        }
                        SaveBase64ToFile(financial.LatestBankStatementPath, filepath);
                        financial.LatestBankStatementPath = filepath;
                    }
                    // else: already a stored file path, keep as-is
                }


                financial.VendorId = vendorId;

                var existing = await _context.VendorFinancials
                    .FirstOrDefaultAsync(x => x.VendorId == vendorId);

                if (existing == null)
                {
                    await _context.VendorFinancials.AddAsync(financial);
                }
                else
                {
                    financial.Id = existing.Id; // Ensure we keep the same PK for related entities
                    _context.Entry(existing).CurrentValues.SetValues(financial);
                }

                // ----- VendorCreditFacility (upsert + sync) -----
                var creditSet = _context.Set<VendorCreditFacility>();
                var existingCredits = await creditSet.Where(x => x.VendorId == vendorId).ToListAsync();

                var incomingCredits = financial.CreditFacilities ?? Enumerable.Empty<VendorCreditFacility>();
                foreach (var incoming in incomingCredits)
                {
                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existingCredits.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        else
                            await creditSet.AddAsync(incoming);
                    }
                    else
                    {
                        await creditSet.AddAsync(incoming);
                    }
                }

                var incomingCreditIds = incomingCredits.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                var toRemoveCredits = existingCredits.Where(e => !incomingCreditIds.Contains(e.Id)).ToList();
                if (toRemoveCredits.Any()) creditSet.RemoveRange(toRemoveCredits);

                // ----- VendorTax (single upsert or remove) -----
                var taxSet = _context.Set<VendorTax>();
                var existingTax = await taxSet.FirstOrDefaultAsync(x => x.VendorId == vendorId);

                if (financial.Tax != null)
                {
                    financial.Tax.VendorId = vendorId;
                    if (existingTax == null)
                    {
                        await taxSet.AddAsync(financial.Tax);
                    }
                    else
                    {
                        financial.Tax.Id = existingTax.Id;
                        _context.Entry(existingTax).CurrentValues.SetValues(financial.Tax);
                    }
                }
                else
                {
                    // If incoming tax is null, remove existing tax (synchronise)
                    if (existingTax != null)
                        taxSet.Remove(existingTax);
                }

                // ----- VendorBank (single upsert or remove) -----
                var bankSet = _context.Set<VendorBank>();
                var existingBank = await bankSet.FirstOrDefaultAsync(x => x.VendorId == vendorId);

                if (financial.Bank != null)
                {
                    // Save bank statement attachment (base64 → physical file)
                    if (!string.IsNullOrEmpty(financial.Bank.Attachment))
                    {
                        // Heuristic: if contains ',' (data URI) or is long (likely base64), attempt to save.
                        if (financial.Bank.Attachment.Contains(",") || financial.Bank.Attachment.Length > 200)
                        {
                            var uploadPath = _configuration["FileSettings:UploadPath"];
                            string attachmentPath = Path.Combine(uploadPath, $"vendor_{vendorId}_BankStatement.pdf");
                            if (File.Exists(attachmentPath))
                                File.Delete(attachmentPath);
                            SaveBase64ToFile(financial.Bank.Attachment, attachmentPath);
                            financial.Bank.Attachment = attachmentPath;
                        }
                        // else: already a stored file path, keep as-is
                    }

                    financial.Bank.VendorId = vendorId;
                    if (existingBank == null)
                    {
                        await bankSet.AddAsync(financial.Bank);
                    }
                    else
                    {
                        financial.Bank.Id = existingBank.Id;
                        _context.Entry(existingBank).CurrentValues.SetValues(financial.Bank);
                    }
                }
                else
                {
                    // If incoming bank is null, remove existing bank (synchronise)
                    if (existingBank != null)
                        bankSet.Remove(existingBank);
                }

                // Persist all changes in one Save
                await _context.SaveChangesAsync();

                // mark Financial step completed
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Financial);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Financial);
        }

        #endregion

        #region SAVE CATEGORIES (Validation Added)

        //public async Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, List<VendorCategory> categories,List<VendorCategoryCertificate> VendorCategoryCertificate)
        //{
        //    if (categories.Count > 2)
        //        throw new Exception("Maximum 2 main category codes allowed.");

        //    using var tx = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var existing = await _context.VendorCategories.Where(x => x.VendorId == vendorId).ToListAsync();

        //        // upsert incoming categories
        //        foreach (var incoming in categories)
        //        {
        //            incoming.VendorId = vendorId;
        //            if (incoming.Id > 0)
        //            {
        //                var exist = existing.FirstOrDefault(x => x.Id == incoming.Id);
        //                if (exist != null)
        //                    _context.Entry(exist).CurrentValues.SetValues(incoming);
        //                else
        //                    await _context.VendorCategories.AddAsync(incoming);
        //            }
        //            else
        //            {


        //                //if (!string.IsNullOrEmpty(incoming.CertificatePath))
        //                //{
        //                //    var uploadRoot = _configuration["FileSettings:UploadPath"];

        //                //    var folderPath = Path.Combine(uploadRoot, "CategoryCodeCertificate");

        //                //    // Create directory if not exists
        //                //    if (!Directory.Exists(folderPath))
        //                //    {
        //                //        Directory.CreateDirectory(folderPath);
        //                //    }

        //                //    var filePath = Path.Combine(folderPath, $"vendor_{vendorId}_Certificate.pdf");

        //                //    if (File.Exists(filePath))
        //                //    {
        //                //        File.Delete(filePath);
        //                //    }

        //                //    SaveBase64ToFile(incoming.CertificatePath, filePath);

        //                //    // Save relative path instead of full path
        //                //    incoming.CertificatePath = Path
        //                //        .GetRelativePath(uploadRoot, filePath)
        //                //        .Replace("\\", "/");
        //                //}


        //                await _context.VendorCategories.AddAsync(incoming);
        //            }
        //        }

        //        // remove categories not present in incoming (synchronise)
        //        var incomingIds = categories.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
        //        var toRemove = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
        //        if (toRemove.Any()) _context.VendorCategories.RemoveRange(toRemove);

        //        await _context.SaveChangesAsync();

        //        // mark Category step completed
        //        await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Category);

        //        await tx.CommitAsync();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        throw;
        //    }
        //    return ComputeNextStep(VendorRegistrationStep.Category);
        //}



        public async Task<VendorRegistrationStep?> SaveCategoriesAsync(int vendorId, List<VendorCategory> categories, List<VendorCategoryCertificate> vendorCategoryCertificates)
        {
            categories ??= new List<VendorCategory>();
            vendorCategoryCertificates ??= new List<VendorCategoryCertificate>();

            //if (categories.Count > 2)
            //    throw new Exception("Maximum 2 main category codes allowed.");

            //var subCategoryValidation = categories
            //                    .GroupBy(x => x.CategoryId)
            //                    .Where(g => g.Select(x => x.SubCategoryId)
            //                    .Distinct()
            //                    .Count() > 3)
            //                    .ToList();

            //if (subCategoryValidation.Any())
            //{
            //    throw new Exception("Maximum three (3) SubCategoryId allowed per CategoryId.");
            //}

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // ----- CATEGORIES (upsert + sync) -----
                var existing = await _context.VendorCategories.Where(x => x.VendorId == vendorId).ToListAsync();

                // upsert incoming categories
                foreach (var incoming in categories)
                {

                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existing.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        else
                            await _context.VendorCategories.AddAsync(incoming);
                    }
                    else
                    {
                        await _context.VendorCategories.AddAsync(incoming);
                    }
                }

                // remove categories not present in incoming (synchronise)
                var incomingIds = categories.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                var toRemove = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
                if (toRemove.Any()) _context.VendorCategories.RemoveRange(toRemove);

                // ----- CERTIFICATES (upsert + sync) -----
                var certSet = _context.Set<VendorCategoryCertificate>();
                var existingCerts = await certSet.Where(x => x.VendorId == vendorId).ToListAsync();

                // prepare upload folder
                var uploadRoot = _configuration["FileSettings:UploadPath"];
                if (string.IsNullOrWhiteSpace(uploadRoot))
                {
                    uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                }
                var certFolder = Path.Combine(uploadRoot, "CategoryCodeCertificate");
                if (!Directory.Exists(certFolder))
                    Directory.CreateDirectory(certFolder);

                foreach (var incomingCert in vendorCategoryCertificates)
                {
                    incomingCert.VendorId = vendorId;

                    // If certificate path is non-empty and looks like base64, save to disk and store relative path
                    if (!string.IsNullOrEmpty(incomingCert.CertificatePath))
                    {
                        // Heuristic: if contains ',' (data URI) or is long (likely base64), attempt to save.
                        if (incomingCert.CertificatePath.Contains(",") || incomingCert.CertificatePath.Length > 200)
                        {
                            var safeFileName = $"vendor_{vendorId}_codemaster_{incomingCert.CodeMasterId}_{Guid.NewGuid():N}.pdf";
                            var fullPath = Path.Combine(certFolder, safeFileName);

                            // overwrite if same file exists is unlikely due to GUID, but remove if present
                            if (File.Exists(fullPath))
                                File.Delete(fullPath);

                            SaveBase64ToFile(incomingCert.CertificatePath, fullPath);

                            // save relative path for DB
                            incomingCert.CertificatePath = Path.GetRelativePath(uploadRoot, fullPath).Replace("\\", "/");
                        }
                        // else assume it's already a stored relative path or URL and keep as-is
                    }

                    if (incomingCert.Id > 0)
                    {
                        var exist = existingCerts.FirstOrDefault(x => x.Id == incomingCert.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incomingCert);
                        else
                            await certSet.AddAsync(incomingCert);
                    }
                    else
                    {
                        await certSet.AddAsync(incomingCert);
                    }
                }

                var incomingCertIds = vendorCategoryCertificates.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                var toRemoveCerts = existingCerts.Where(e => !incomingCertIds.Contains(e.Id)).ToList();
                if (toRemoveCerts.Any()) certSet.RemoveRange(toRemoveCerts);

                // Persist all changes
                await _context.SaveChangesAsync();

                // Note: Category change logging moved to CategoryCodeApprovalRepository.ApproveRequestAsync()
                // to ensure only approved requests are counted toward the change limit.
                // This method is now used only for initial registration (before SAP approval).

                // mark Category step completed
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Category);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Category);
        }








        #endregion

        #region SAVE EXPERIENCE

        public async Task<VendorRegistrationStep?> SaveExperiencesAsync(int vendorId, List<VendorExperience> experiences)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.VendorExperiences.Where(x => x.VendorId == vendorId).ToListAsync();

                foreach (var incoming in experiences)
                {

                    if (!string.IsNullOrEmpty(incoming.AttachmentPath))
                    {

                        var uploadRoot = _configuration["FileSettings:UploadPath"];
                        if (string.IsNullOrWhiteSpace(uploadRoot))
                        {
                            uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                        }
                        var certFolder = Path.Combine(uploadRoot, "Experience");
                        if (!Directory.Exists(certFolder))
                            Directory.CreateDirectory(certFolder);



                        // Heuristic: if contains ',' (data URI) or is long (likely base64), attempt to save.
                        if (incoming.AttachmentPath.Contains(",") || incoming.AttachmentPath.Length > 200)
                        {
                            var safeFileName = $"vendor_{vendorId}_Experience_{Guid.NewGuid():N}.pdf";
                            var fullPath = Path.Combine(certFolder, safeFileName);

                            // overwrite if same file exists is unlikely due to GUID, but remove if present
                            if (File.Exists(fullPath))
                                File.Delete(fullPath);

                            SaveBase64ToFile(incoming.AttachmentPath, fullPath);

                            // save relative path for DB
                            incoming.AttachmentPath = Path.GetRelativePath(uploadRoot, fullPath).Replace("\\", "/");
                        }
                        // else assume it's already a stored relative path or URL and keep as-is
                    }


                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existing.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                        {
                            incoming.Id = exist.Id; // Ensure we keep the same PK for related entities
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        }
                        else
                        {
                            await _context.VendorExperiences.AddAsync(incoming);
                        }
                    }
                    else
                    {
                        await _context.VendorExperiences.AddAsync(incoming);
                    }
                }

                var incomingIds = experiences.Where(e => e.Id > 0).Select(e => e.Id).ToHashSet();
                var toRemove = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
                if (toRemove.Any()) _context.VendorExperiences.RemoveRange(toRemove);

                await _context.SaveChangesAsync();

                // mark Experience step completed
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Experience);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Experience);
        }

        #endregion

        #region SAVE DECLARATION

        public async Task<VendorRegistrationStep?> SaveDeclarationAsync(int vendorId, VendorDeclaration declaration)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                declaration.VendorId = vendorId;

                var existing = await _context.VendorDeclarations
                    .FirstOrDefaultAsync(x => x.VendorId == vendorId);

                if (existing == null)
                {
                    await _context.VendorDeclarations.AddAsync(declaration);
                }
                else
                {
                    declaration.Id = existing.Id;
                    _context.Entry(existing).CurrentValues.SetValues(declaration);
                }

                await _context.SaveChangesAsync();

                // mark Declaration step completed
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Declaration);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return ComputeNextStep(VendorRegistrationStep.Declaration);
        }

        #endregion

        #region SAVE PAYMENT

        // Example payment save - enable if needed and advance step to Payment
        //public async Task SavePaymentAsync(int vendorId, VendorPayment payment)
        //{
        //    using var tx = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        payment.VendorId = vendorId;
        //        payment.PaidDate = DateTime.UtcNow;
        //        payment.PaymentStatus = "Paid";
        //
        //        await _context.VendorPayments.AddAsync(payment);
        //        await _context.SaveChangesAsync();
        //
        //        await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Payment);
        //
        //        await tx.CommitAsync();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        throw;
        //    }
        //}

        #endregion


        //    var vendor = await _context.Vendors
        //.Include(v => v.Categories)
        //    .ThenInclude(c => c.MasterCategory)
        //        .ThenInclude(m => m.Parent)
        //.FirstOrDefaultAsync(v => v.Id == vendorId);

        public async Task<VendorRegistrationStep?> SaveVendorCategoryAsync(int vendorId, VendorCategory category)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                category.VendorId = vendorId;

                var existing = await _context.VendorCategories
                    .FirstOrDefaultAsync(x =>
                        x.VendorId == vendorId
                        //&& x.MasterCategoryId == category.MasterCategoryId
                        );

                if (existing == null)
                {
                    await _context.VendorCategories.AddAsync(category);
                }
                else
                {
                    // Update existing record
                    _context.Entry(existing).CurrentValues.SetValues(category);
                }

                await _context.SaveChangesAsync();

                // Mark Category step completed
                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Category);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Category);
        }

        public async Task<VendorRegistrationStep?> SaveVendorCategoriesAsync(int vendorId, List<VendorCategory> categories)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.VendorCategories
                    .Where(x => x.VendorId == vendorId).ToListAsync();

                // upsert incoming
                foreach (var incoming in categories)
                {
                    incoming.VendorId = vendorId;
                    if (incoming.Id > 0)
                    {
                        var exist = existing.FirstOrDefault(x => x.Id == incoming.Id);
                        if (exist != null)
                            _context.Entry(exist).CurrentValues.SetValues(incoming);
                        else
                            await _context.VendorCategories.AddAsync(incoming);
                    }
                    else
                    {
                        await _context.VendorCategories.AddAsync(incoming);
                    }
                }

                var incomingIds = categories.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();
                var toRemove = existing.Where(e => !incomingIds.Contains(e.Id)).ToList();
                if (toRemove.Any()) _context.VendorCategories.RemoveRange(toRemove);

                await _context.SaveChangesAsync();

                await AdvanceStepIfNeededAsync(vendorId, VendorRegistrationStep.Category);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            return ComputeNextStep(VendorRegistrationStep.Category);
        }

        public async Task SaveSSMResponse(string input, string response)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {

                SSMResponse res = new SSMResponse();
                res.Input = input;
                res.Response = response;
                res.ResponseDateTime = DateTime.Now;
                _context.SSMResponses.Add(res);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsRocNumberExistsAsync(string rocNumber)
        {
            return await _context.Vendors.AnyAsync(v => v.RocNumber == rocNumber);
        }

        public async Task<VendorProfileDto> RegisterVendor(Vendor vendor)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // initialize vendor defaults for registration flow
                vendor.CreatedDate = DateTime.UtcNow;

                vendor.VendorCodeStatus = VendorStatus.Draft.GetDisplayName();
                vendor.CurrentStep = VendorRegistrationStep.CreateAccount;
                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw new Exception("Error while saving record");
            }
            return await GetVendorById(vendor.Id);
        }


        public async Task<VendorProfileDto> GetVendorById(int vendorId)
        {
            try
            {
                var vendor = await _context.Vendors
                    .Include(v => v.VendorCategories)
                        .ThenInclude(c => c.Category)
                            .ThenInclude(m => m.SubCategories).ThenInclude(x => x.Activities)
                    .FirstOrDefaultAsync(v => v.Id == vendorId);
                return _mapper.Map<VendorProfileDto>(vendor);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrive record");
            }
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


        public async Task<PaymentDetailsDTO> GetPaymentDetailsAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor != null)
            {
                var paymentDetailsDTO = new PaymentDetailsDTO()
                {
                    PaymentDescription = vendor.RocNumber + vendor.CompanyName,
                    Email = vendor.Email,
                    Amount = 150.00
                };
                return paymentDetailsDTO;
            }
            return new PaymentDetailsDTO();


        }


        public async Task<IEnumerable<CompanyCategoryDto>> GetCompanyTypes()
        {
            var companyTypes = await _context.companyCategories.Include(x => x.CompanyEntityType).ToListAsync();
            return _mapper.Map<IEnumerable<CompanyCategoryDto>>(companyTypes);

        }

        public async Task<IEnumerable<CompanyCategoryDto>> GetCompanyEntitiesByTypeIdAsync(int TypeId)
        {
            var companyTypes = await _context.companyCategories.Where(x => x.Id == TypeId).Include(x => x.CompanyEntityType).ToListAsync();
            return _mapper.Map<IEnumerable<CompanyCategoryDto>>(companyTypes);

        }

        public async Task<IEnumerable<VendorProfileDto>> GetVendorListAsync()
        {
            var companyTypes = await _context.Vendors
                .Where(v => v.IsActive)
                .Include(x => x.CompanyEntityType)
                .ToListAsync();
            return _mapper.Map<IEnumerable<VendorProfileDto>>(companyTypes);

        }


        public async Task<VendorProfileDto> GetVendorByVendorIdAsync(int vendorId)
        {
            var vendor = await _context.Vendors.Where(x => x.Id == vendorId).FirstOrDefaultAsync();
            return _mapper.Map<VendorProfileDto>(vendor);

        }

        //csharp DB\Repositories\VendorRepository.cs
        public async Task<Vendor?> GetSAPVendorByVendorIdAsync(int vendorId)
        {
            try
            {
                // Increase command timeout for this read if needed (seconds).
                // Remove or lower in production if you don't want a longer DB timeout.
                _context.Database.SetCommandTimeout(60);

                // Return a no-tracking, eagerly loaded Vendor so caller can read nested props
                // after this method returns without touching a disposed DbContext.
                return await _context.Vendors
                    .AsNoTracking()
                    .Include(v => v.CompanyEntityType)
                    .Include(v => v.IndustryType)
                    .Include(v => v.VendorFinancial)
                        .ThenInclude(f => f.Bank)
                    .Include(v => v.VendorFinancial)
                        .ThenInclude(f => f.Tax)
                    .Include(v => v.VendorBanks)
                    .Include(v => v.VendorTaxes)
                    .FirstOrDefaultAsync(v => v.Id == vendorId);
            }
            catch (Exception ex)
            {
                // Re-throw with context (or log as needed). Avoid swallowing exceptions.
                throw new Exception($"Error fetching vendor {vendorId} for SAP call: {ex.Message}", ex);
            }
        }

        public VendorProfileDto GetVendorByROCandPasswordAsync(string roc, string password)
        {
            var vendor = _context.Vendors.Where(x => x.RocNumber == roc && x.PasswordHash == password && x.IsActive).FirstOrDefault();
            return _mapper.Map<VendorProfileDto>(vendor);

        }

        public async Task<DateTime?> UpdateLastLoginAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null)
                return null;

            var previous = vendor.LastLogin;
            vendor.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return previous;
        }

        public async Task<VendorDashboardDto> GetVendorDashboardAsync()
        {
            var vendors = await _context.Vendors
                .Where(v => v.IsActive)
                .Include(x => x.CompanyEntityType)
                .ToListAsync();

            var mappedVendors = _mapper.Map<IEnumerable<VendorProfileDto>>(vendors);

            var dashboard = new VendorDashboardDto
            {
                Total = vendors.Count,
                Draft = vendors.Count(x => x.VendorCodeStatus == VendorStatus.Draft.GetDisplayName()),
                PendingApproval = vendors.Count(x => x.VendorCodeStatus == VendorStatus.PendingApproval.GetDisplayName()),
                Approved = vendors.Count(x => x.VendorCodeStatus == VendorStatus.Approved.GetDisplayName()),
                Expired = vendors.Count(x => x.VendorCodeStatus == VendorStatus.Expired.GetDisplayName()),
                Blacklisted = vendors.Count(x => x.VendorCodeStatus == VendorStatus.Blacklisted.GetDisplayName()),

                Vendors = mappedVendors
            };

            return dashboard;
        }

        public async Task<IEnumerable<VendorProfileDto>> GetVendorListAsync(VendorSearchRequest? request)
        {
            var query = _context.Vendors
                .Where(v => v.IsActive)
                .Include(x => x.CompanyEntityType)
                .AsQueryable();

            if (request != null)
            {
                if (request.VendorTypeId.HasValue)
                    query = query.Where(x => x.CompanyEntityTypeId == request.VendorTypeId.Value);

                if (request.StateId.HasValue)
                    query = query.Where(x => x.StateId == request.StateId);

                if (request.VendorCodeStatusId.HasValue)
                    query = query.Where(x => x.VendorCodeStatus == EnumExtensions.GetDisplayNameFromInt<VendorStatus>(request.VendorCodeStatusId.Value));


            }

            var result = await query.ToListAsync();
            return _mapper.Map<IEnumerable<VendorProfileDto>>(result);
        }

        public async Task SaveSAPRequestResponseAsync(int VendorId, string request, string response)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var sapRes = new Vendor_SAPRequestResponse
                {
                    VendorId = VendorId,
                    Request = request,
                    Response = response,
                    ResponseDateTime = DateTime.UtcNow
                };
                await _context.Vendor_SAPRequestResponses.AddAsync(sapRes);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Vendor>> GetVendorsWithExpiryOnAsync(DateTime targetDateUtc)
        {
            var targetDate = targetDateUtc.Date;
            var approvedDisplay = VendorStatus.Approved.GetDisplayName();

            return await _context.Vendors
                .Where(v => v.IsActive
                            && v.VendorCodeStatus == approvedDisplay
                            && v.RegistrationExpiryDate.HasValue
                            && v.RegistrationExpiryDate.Value.Date == targetDate)
                .ToListAsync();
        }

        public async Task<bool> HasRenewalReminderBeenSentAsync(int vendorId, int thresholdDays, DateTime expiryDate)
        {
            var expiry = expiryDate.Date;
            return await _context.VendorReminderLogs
                .AnyAsync(l => l.VendorId == vendorId
                            && l.ThresholdDays == thresholdDays
                            && l.ExpiryDate == expiry);
        }

        public async Task RecordRenewalReminderSentAsync(int vendorId, int thresholdDays, DateTime expiryDate)
        {
            _context.VendorReminderLogs.Add(new VendorReminderLog
            {
                VendorId = vendorId,
                ThresholdDays = thresholdDays,
                ExpiryDate = expiryDate.Date,
                SentAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task<Vendor?> RenewRegistrationAsync(int vendorId, DateTime renewalUtc, DateTime newExpiryUtc)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null) return null;

            vendor.RegistrationExpiryDate = newExpiryUtc;
            vendor.LastRenewedOn = renewalUtc;

            // If status had flipped to Expired, restore to Approved on successful renewal
            if (vendor.VendorCodeStatus == VendorStatus.Expired.GetDisplayName())
            {
                vendor.VendorCodeStatus = VendorStatus.Approved.GetDisplayName();
            }

            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<bool> MarkVendorApprovedAsync(int vendorId, string vendorCode, DateTime approvalUtc)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null) return false;

            var approvedDisplay = VendorStatus.Approved.GetDisplayName();

            // Idempotent: if vendor is already marked Approved with the same code, do nothing
            if (vendor.VendorCodeStatus == approvedDisplay
                && !string.IsNullOrWhiteSpace(vendor.VendorCode)
                && vendor.VendorCode == vendorCode)
            {
                return false;
            }

            vendor.VendorCode = vendorCode;
            vendor.VendorCodeStatus = approvedDisplay;
            vendor.ApprovalDatetime = approvalUtc;
            vendor.RegistrationExpiryDate = approvalUtc.AddYears(3);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<IndustryTypeDto>> BindIndustryTypeListAsync()
        {
            var companyTypes = await _context.IndustryTypes.ToListAsync();
            return _mapper.Map<IEnumerable<IndustryTypeDto>>(companyTypes);

        }

        public async Task SaveQuestionAnswers(int vendorId, List<QuestionAnswerDto> answers)
        {
            foreach (var item in answers)
            {
                var entity = new QuestionAnswer
                {
                    VendorId = vendorId,
                    QuestionId = item.QuestionId,
                    Answer = item.Answer,
                    AnswerDate = DateTime.UtcNow
                };

                _context.QuestionAnswers.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<QuestionAnswerDto>> GetQuestionAnswersByQuestionnaireId(int questionnaireId, int vendorId)
        {
            var result = await _context.Questions
                .Where(q => q.QuestionnaireId == questionnaireId)
                .Select(q => new QuestionAnswerDto
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionType=q.QuestionType,
                    Answer = _context.QuestionAnswers
                        .Where(a => a.QuestionId == q.Id && a.VendorId == vendorId)
                        .Select(a => a.Answer)
                        .FirstOrDefault()
                })
                .OrderBy(q => q.QuestionId)
                .ToListAsync();

            return result;
        }

        #region CATEGORY CHANGE TRACKING

        public async Task<int> GetCategoryChangeCountAsync(int vendorId, DateTime from, DateTime to)
        {
            return await _context.VendorCategoryChangeLogs
                .Where(l => l.VendorId == vendorId && l.ChangeDate >= from && l.ChangeDate <= to)
                .CountAsync();
        }

        public async Task LogCategoryChangeAsync(int vendorId, string description)
        {
            var log = new VendorCategoryChangeLog
            {
                VendorId = vendorId,
                ChangeDate = DateTime.UtcNow,
                ChangeDescription = description,
                CreatedBy = GetCurrentUserId(),
                CreatedDate = DateTime.UtcNow
            };
            await _context.VendorCategoryChangeLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        #endregion

    }

}