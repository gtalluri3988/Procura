using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations.Helpers;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DB.Repositories
{
    public class ResidentRepository : RepositoryBase<Resident, ResidentDTO>, IResidentRepository
    {
        public ResidentRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<IEnumerable<ResidentDTO>> GetAllResidentsByCommunityIdAsync(int communityId)
        {
            //var residents = await _context.Resident.Include(c => c.State).Include(c=>c.ResidencePaymentDetails).Where(x => x.CommunityId == communityId).ToListAsync();
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var residents = await _context.Resident.Where(x => x.Community != null && x.Community.Status == true)
                .Include(r => r.State)
                .Include(r => r.ResidencePaymentDetails)
                    .ThenInclude(p => p.PaymentStatus)
                .Where(r => r.CommunityId == communityId).OrderBy(x => x.Name)
                .ToListAsync();
            // Map all residents
            var residentDtos = _mapper.Map<List<ResidentDTO>>(residents);
            // Attach PaymentStatus manually
            foreach (var dto in residentDtos)
            {
                var entity = residents.First(r => r.Id == dto.Id);
                var payment = entity.ResidencePaymentDetails?
                    .FirstOrDefault(p =>
                        p.PaymentDate.HasValue &&
                        p.PaymentDate.Value.Month == currentMonth &&
                        p.PaymentDate.Value.Year == currentYear);
                dto.PaymentStatus = payment?.PaymentStatus?.Name ?? "Awaiting Payment";
            }
            return residentDtos;
        }
        public async Task<IEnumerable<ResidentDTO>> SearchResidentsByCommunityIdAsync(ResidentDTO searchModel)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            // Start with base query
            var query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true)
                .Include(r => r.State)
                .Include(r => r.ResidencePaymentDetails)
                    .ThenInclude(p => p.PaymentStatus)
                .Where(r => r.CommunityId == searchModel.CommunityId);
            // Apply filters only if values are provided
            if (!string.IsNullOrWhiteSpace(searchModel.RoadNo))
                query = query.Where(r => r.RoadNo == searchModel.RoadNo);
            if (!string.IsNullOrWhiteSpace(searchModel.BlockNo))
                query = query.Where(r => r.BlockNo == searchModel.BlockNo);
            if (!string.IsNullOrWhiteSpace(searchModel.HouseNo))
                query = query.Where(r => r.HouseNo == searchModel.HouseNo);
            if (!string.IsNullOrWhiteSpace(searchModel.Level))
                query = query.Where(r => r.Level == searchModel.Level);
            var residents = await query.ToListAsync();
            var residentDtos = _mapper.Map<List<ResidentDTO>>(residents);
            foreach (var dto in residentDtos)
            {
                var entity = residents.First(r => r.Id == dto.Id);
                var payment = entity.ResidencePaymentDetails?
                    .FirstOrDefault(p =>
                        p.PaymentDate.HasValue &&
                        p.PaymentDate.Value.Month == currentMonth &&
                        p.PaymentDate.Value.Year == currentYear);
                dto.PaymentStatus = payment?.PaymentStatus?.Name ?? "Awaiting Payment";
            }
            if (!string.IsNullOrWhiteSpace(searchModel.maintainanceFee))
                residentDtos = residentDtos.Where(x => x.PaymentStatus == searchModel.maintainanceFee).ToList();
            return residentDtos;
        }
        public async Task<ResidentDTO> SaveResidentAsync(ResidentDTO? resident)
        {
            var entity = _mapper.Map<EFModel.Resident>(resident);
            var residentCheck = _context.Resident.Where(x => x.RoadNo == resident.RoadNo && x.BlockNo == resident.BlockNo
            && x.Level == resident.Level && x.HouseNo == resident.HouseNo && x.CommunityId == resident.CommunityId).FirstOrDefault();
            if (residentCheck != null)
            {
                throw new Exception("Already another resident alloted for this unit");
            }
            if (resident != null && CheckEmailExist(resident.Email, resident.CommunityId))
            {
                throw new Exception("Email address has been registered. Please use a different email address");
            }
            entity.RoleId = 5;
            entity.IsFirstTimeLogin= true;
            _context.Resident.Add(entity);
            await _context.SaveChangesAsync();
            if (resident != null && !string.IsNullOrWhiteSpace(resident.Email))
            {
                string communityFullName = string.Empty;
                var password = EmailHelper.GenerateRandomPassword();
                var res = _context.Resident.Where(x => x.Id == entity.Id).FirstOrDefault();
                if (res != null)
                {
                    res.Password = password;
                    res.IsFirstTimeLogin = true;
                    _context.SaveChanges();
                    var community = _context.Community.Where(x => x.Id == res.CommunityId).FirstOrDefault();
                    if (community != null)
                    {
                        communityFullName =  community?.CommunityName;
                    }
                }
                try
                {
                    await SendWelcomeEmailAsync(
                        toEmail: resident.Email,
                        residentFullName: resident.Name ?? "Resident",
                        tempPassword: password,
                        residentPageUrl: "http://103.27.86.226/ResidentApp/login",
                        communityFullName
                    );
                }
                catch(Exception ex)
                { 
                
                }
            }
            return await GetByIdAsync(entity.Id);
        }
        private Boolean CheckEmailExist(string email,int communityId)
        {
            var resident = _context.Resident.Where(x => x.Email == email && x.CommunityId==communityId && x.Community.Status==true).FirstOrDefault();
            if (resident!=null)
            {
                return true;
            }
            return false;
        }
        public async Task<List<ResidentDTO>> GetResidentsDropdownsAsync(int communityId, string Type)
        {
            try
            {
                IQueryable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
                if (communityId != 0)
                {
                    query = query.Where(x => x.CommunityId == communityId);
                }
                var residents = (await query
                 .Select(x => new ResidentDTO
                 {
                     BlockNo = x.BlockNo,
                     HouseNo = x.HouseNo,
                     Level = x.Level,
                     RoadNo = x.RoadNo,
                 })
                 .ToListAsync())
                 .OrderBy(x =>
                 {
                     return int.TryParse(x.RoadNo, out var n) ? n : int.MaxValue;
                 })
                 .ToList();
                return residents;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<string>> GetRoardsByCommunityAsync(int communityId)
        {
            IQueryable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
            if (communityId != 0)
            {
                query = query.Where(x => x.CommunityId == communityId);
            }
            var roadList = await query
                .Select(x => x.RoadNo)
                .Distinct()
                .ToListAsync();
            return roadList;
        }
        public async Task<List<string>> GetBlocksByRoadAsync(int communityId, string roadNo)
        {
            IQueryable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
            if (communityId != 0)
            {
                query = query.Where(x => x.CommunityId == communityId);
            }
            if (!string.IsNullOrEmpty(roadNo))
            {
                query = query.Where(x => x.RoadNo == roadNo);
            }
            var blockList = await query
                .Select(x => x.BlockNo)
                .Distinct()
                .ToListAsync();
            return blockList;
        }
        public async Task<List<string>> GetLevelsByRoadAsync(int communityId, string roadNo, string blockNo)
        {
            IQueryable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
            if (communityId != 0)
            {
                query = query.Where(x => x.CommunityId == communityId);
            }
            if (!string.IsNullOrEmpty(roadNo))
            {
                query = query.Where(x => x.RoadNo == roadNo && x.BlockNo == blockNo);
            }
            var levelList = await query
                .Select(x => x.Level)
                .Distinct()
                .ToListAsync();
            return levelList;
        }
        public async Task<List<string>> GetHouseNosByLevelAsync(int communityId, string roadNo, string blockNo, string level)
        {
            IQueryable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
            if (communityId != 0)
            {
                query = query.Where(x => x.CommunityId == communityId);
            }
            if (!string.IsNullOrEmpty(roadNo))
            {
                query = query.Where(x => x.RoadNo == roadNo && x.BlockNo == blockNo && x.Level == level);
            }
            var houseList = await query
                .Select(x => x.HouseNo)
                .Distinct()
                .ToListAsync();
            return houseList;
        }
        public async Task<List<string>> GetResidentHierarchyAsync(
        int communityId = 0,
        string roadNo = null,
        string blockNo = null,
        string level = null,
        string targetField = "RoadNo")
        {
            IEnumerable<Resident> query = _context.Resident.Where(x => x.Community != null && x.Community.Status == true);
            if (communityId != 0)
                query = query
                        .Where(x => x.CommunityId == communityId)
                        .AsEnumerable() // switch to in-memory
                        .OrderBy(x =>
                        {
                            var str = x.RoadNo ?? "";
                            var digits = str.TakeWhile(char.IsDigit).ToArray();
                            var numPart = new string(digits);
                            return int.TryParse(numPart, out var n) ? n : int.MaxValue;
                        })
                        .ThenBy(x => x.RoadNo)
                        .ToList();
            if (!string.IsNullOrEmpty(roadNo))
            {
                query = query
                        .Where(x => x.RoadNo == roadNo)
                        .AsEnumerable() // switch to in-memory
                        .OrderBy(x =>
                        {
                            var str = x.BlockNo ?? "";
                            var digits = str.TakeWhile(char.IsDigit).ToArray();
                            var numPart = new string(digits);
                            return int.TryParse(numPart, out var n) ? n : int.MaxValue;
                        })
                        .ThenBy(x => x.BlockNo)
                        .ToList();
            }
            if (!string.IsNullOrEmpty(blockNo))
                query = query.Where(x => x.BlockNo == blockNo).AsEnumerable() // switch to in-memory
                        .OrderBy(x =>
                        {
                            var str = x.Level ?? "";
                            var digits = str.TakeWhile(char.IsDigit).ToArray();
                            var numPart = new string(digits);
                            return int.TryParse(numPart, out var n) ? n : int.MaxValue;
                        })
                        .ThenBy(x => x.Level)
                        .ToList();
            if (!string.IsNullOrEmpty(level))
                query = query.Where(x => x.Level == level).AsEnumerable() // switch to in-memory
                        .OrderBy(x =>
                        {
                            var str = x.HouseNo ?? "";
                            var digits = str.TakeWhile(char.IsDigit).ToArray();
                            var numPart = new string(digits);
                            return int.TryParse(numPart, out var n) ? n : int.MaxValue;
                        })
                        .ThenBy(x => x.HouseNo)
                        .ToList();
            return targetField switch
            {
                "RoadNo" => query.Select(x => x.RoadNo).Distinct().ToList(),
                "BlockNo" => query.Select(x => x.BlockNo).Distinct().ToList(),
                "Level" => query.Select(x => x.Level).Distinct().ToList(),
                "HouseNo" => query.Select(x => x.HouseNo).Distinct().ToList(),
                _ => new List<string>()
            };
        }
        public async Task<ResidentDTO?> GetResidentsByResidentIdAsync(int residentId)
        {
            if (residentId <= 0) return null;

            // 1) Load Resident with related Community and VehicleDetails in one DB call
            var residentEntity = await _context.Resident
                .Include(r => r.Community)
                .Include(r => r.VehicleDetails)
                .Include(r=>r.ResidentPhotos)
                .FirstOrDefaultAsync(r => r.Id == residentId && r.Community != null && r.Community.Status == true);

            if (residentEntity == null) return null;

            // 2) Map to DTO (without embedding images yet)
            var dto = new ResidentDTO
            {
                Id = residentEntity.Id,
                StateId = residentEntity.StateId,
                HouseNo = residentEntity.HouseNo ?? "",
                Name = residentEntity.Name,
                LotNo = residentEntity.LotNo ?? "",
                BlockNo = residentEntity.BlockNo ?? "",
                RoadNo = residentEntity.RoadNo ?? "",
                NRIC = residentEntity.NRIC,
                PhoneNo = residentEntity.PhoneNo,
                Email = residentEntity.Email,
                CommunityId = residentEntity.CommunityId,
                ParkingLotQty = residentEntity.ParkingLotQty,
                MaintenenceFeesCost = residentEntity.MaintenenceFeesCost,
                FeeMonthly=residentEntity.Community?.FeesMonthly==null?"0": residentEntity.Community.FeesMonthly.ToString(),
                Password = residentEntity.Password,
                RoleId = residentEntity.RoleId ?? 0,
                ContactPerson1 = residentEntity.ContactPerson1,
                ContactPerson2 = residentEntity.ContactPerson2,
                Level = residentEntity.Level ?? "",
                IsFirstTimeLogin = residentEntity.IsFirstTimeLogin,
                FileName = string.Empty,         // will populate below if file found
                ImagePath = residentEntity.ImagePath,
                CommunityName = residentEntity.Community?.CommunityName ?? string.Empty,
                ResidentImages = residentEntity.ResidentPhotos,
                VehicleDetails = residentEntity.VehicleDetails?
                    .Select(v => new VehicleDetails
                    {
                        Id = v.Id,
                        VehicleNo = v.VehicleNo,
                        VehicleTypeId = v.VehicleTypeId,
                        FileName = v.FileName,
                        ImagePath = v.ImagePath,
                        type = v.type,
                        ResidentId = v.ResidentId
                    }).ToList() ?? new List<VehicleDetails>()
            };

            // 3) Helper to find file & embed as base64 data uri (returns null on error / not found)
            string? TryEmbedImage(string? imageIdentifier, string directory)
            {
                if (string.IsNullOrWhiteSpace(imageIdentifier)) return null;

                try
                {
                    // Find first matching file (case-insensitive contains)
                    var match = Directory.EnumerateFiles(directory)
                                         .FirstOrDefault(f => Path.GetFileName(f)
                                         .IndexOf(imageIdentifier, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (string.IsNullOrEmpty(match) || !File.Exists(match))
                        return null;

                    var bytes = File.ReadAllBytes(match);
                    var ext = Path.GetExtension(match)?.TrimStart('.').ToLowerInvariant() ?? "octet-stream";
                    var base64 = Convert.ToBase64String(bytes);

                    // Ensure correct mime-ish type (basic)
                    var mimeType = ext switch
                    {
                        "jpg" or "jpeg" => "jpeg",
                        "png" => "png",
                        "gif" => "gif",
                        "bmp" => "bmp",
                        "webp" => "webp",
                        _ => "octet-stream"
                    };

                    return $"data:image/{mimeType};base64,{base64}";
                }
                catch
                {
                    // optionally log the exception; return null to indicate failure
                    return null;
                }
            }

            // 4) Populate resident image (if any)
            var residentImageDir = @"C:\Uploads\ResidentVehicles"; // consider moving to config
            var residentDataUri = TryEmbedImage(residentEntity.FileName, residentImageDir);
            if (residentDataUri != null)
            {
                dto.FileName = residentDataUri;
                // optionally set a Name property if DTO has it:
                // dto.Name = Path.GetFileName(matchingFilePath); // need to return path from helper to get filename
            }
            else
            {
                // if not found, keep FileName empty or use residentEntity.FileName as fallback
                dto.FileName = residentEntity.FileName ?? string.Empty;
            }

            // 5) If you want to embed images for each vehicle file, iterate through VehicleDetails
            foreach (var v in dto.VehicleDetails)
            {
                var vehicleDataUri = TryEmbedImage(v.FileName, residentImageDir); // or vehicle-specific dir
                if (vehicleDataUri != null)
                {
                    v.FileName = vehicleDataUri; // replaced with data URI
                }
                // else keep original v.FileName (filename) or set empty
            }

            return dto;
        }


        public ResidentDTO GetResidentsByEmailPasswordAsync(string Email, string Password)
        {
            var checkcomm = _context.Resident.Include(x=>x.Community).Where(x => x.Email == Email && x.Password == Password).FirstOrDefault();
            if(checkcomm==null)
            {
                throw new ArgumentException("Wrong email or password");
            }
            if (checkcomm?.Community?.Status==false || checkcomm?.Community?.Status == null)
                throw new ArgumentException("Resident community not found");

            var residents = _context.Resident.Where(x => x.Email == Email && x.Password == Password && x.Community.Status == true).FirstOrDefault();
            return _mapper.Map<ResidentDTO>(residents);
        }
        public async Task UpdateResidenAsync(int residentId, ResidentDTO resident)
        {
            var entity = await _context.Resident
                .Include(c => c.VehicleDetails)
                .FirstOrDefaultAsync(c => c.Id == residentId);

            if (entity == null)
                return;

            // ============================
            // UPDATE BASIC RESIDENT INFO
            // ============================
            //entity.BlockNo = resident.BlockNo;
            entity.UpdatedDate = DateTime.Now;
            entity.Email = resident.Email;
            entity.Name = resident.Name;
            //entity.ParkingLotQty = resident.ParkingLotQty;
            //entity.HouseNo = resident.HouseNo;
            entity.PhoneNo = resident.PhoneNo;
            //entity.LotNo = resident.LotNo;
            //entity.Level = resident.Level;'
            entity.ContactPerson1=resident.ContactPerson1;
            entity.ContactPerson2=resident.ContactPerson2;
            entity.MaintenenceFeesCost = resident.MaintenenceFeesCost;
            entity.NRIC = resident.NRIC;
            entity.StateId = resident.StateId;
            entity.ParkingLotNos = resident.ParkingLotNos;

            // ============================
            // UPDATE VEHICLE DETAILS
            // ============================
            if (resident.VehicleDetails != null)
            {
                entity.VehicleDetails.Clear();
                foreach (var vehicle in resident.VehicleDetails)
                {
                    entity.VehicleDetails.Add(new VehicleDetails
                    {
                        ResidentId = vehicle.ResidentId,
                        VehicleNo = vehicle.VehicleNo,
                        VehicleTypeId = vehicle.VehicleTypeId,
                        UpdatedDate = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();

            // ============================
            // HANDLE IMAGES
            // ============================

            var existingPhotos = await _context.ResidentPhotos
                .Where(p => p.ResidentId == residentId)
                .ToListAsync();
            int i = 0;
            foreach (var old in existingPhotos)
            {
                
                var p = _context.ResidentPhotos.Where(x => x.Id == old.Id).FirstOrDefault();
                if(p!=null)
                {
                    try
                    {
                        p.ImageLabel = resident.ResidentImages[i].ImageLabel;

                        _context.SaveChanges();
                        i++;
                    }
                    catch(Exception ex)
                    {

                    }
                }
                
            }

            string uploadsPath = @"C:\Uploads\ResidentVehicles";

            // 🔹 CASE: If no images sent → delete all
            if (resident.ResidentImages == null || !resident.ResidentImages.Any())
            {
                foreach (var old in existingPhotos)
                {
                    if (!string.IsNullOrEmpty(old.ImagePath) && File.Exists(old.ImagePath))
                    {
                        try { File.Delete(old.ImagePath); } catch { }
                    }
                    _context.ResidentPhotos.Remove(old);
                }

                await _context.SaveChangesAsync();
                return;
            }

            // ============================
            // DELETE REMOVED IMAGES (MISSING IN FRONTEND)
            // ============================

            var incomingFilenames = resident.ResidentImages
                .Where(i => !string.IsNullOrEmpty(i.Preview))
                .Select(i =>
                {
                    if (i.Preview.StartsWith("http"))
                        return Path.GetFileName(new Uri(i.Preview).LocalPath);
                    return null;
                })
                .Where(f => f != null)
                .ToList();

            foreach (var old in existingPhotos)
            {
                var oldFile = Path.GetFileName(old.ImagePath);

                if (!incomingFilenames.Contains(oldFile))
                {
                    if (File.Exists(old.ImagePath))
                    {
                        try { File.Delete(old.ImagePath); } catch { }
                    }

                    _context.ResidentPhotos.Remove(old);
                }
            }

            await _context.SaveChangesAsync();

            // ============================
            // ADD NEW IMAGES
            // ============================

            foreach (var image in resident.ResidentImages)
            {
                if (string.IsNullOrWhiteSpace(image.Preview))
                    continue;

                if (!string.IsNullOrEmpty(image.ImageLabel))
                {
                    var existing = existingPhotos
                        .FirstOrDefault(p => p.ImageLabel == image.ImageLabel);

                    if (existing != null)
                    {
                        foreach (var old in existingPhotos)
                        {
                            existing.ImageLabel = image.ImageLabel;   // ✅ LABEL UPDATE
                            existing.UpdatedDate = DateTime.Now;
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                // NEW BASE64 IMAGE
                if (image.Preview.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    await AddResidentVehicleSelfieAsync(residentId, image.Preview,image.ImageLabel);
                }
                // EXISTING IMAGE URL → ensure DB record exists
                else if (image.Preview.StartsWith("http"))
                {
                    var fileName = Path.GetFileName(new Uri(image.Preview).LocalPath);
                    var fullPath = Path.Combine(uploadsPath, fileName);

                    if (!existingPhotos.Any(p => p.ImagePath.EndsWith(fileName)))
                    {
                        _context.ResidentPhotos.Add(new ResidentPhotos
                        {
                            ResidentId = residentId,
                            Name = fileName,
                            ImagePath = fullPath,
                            UpdatedDate = DateTime.Now,
                            ImageLabel=image.ImageLabel
                        });
                    }
                    else
                    {

                    }
                }
            }

            await _context.SaveChangesAsync();
        }




        public async Task UpdateResidenByAdminAsync(int residentId, ResidentDTO resident)
        {
            var entity = await _context.Resident
                .Include(c => c.VehicleDetails)
                .FirstOrDefaultAsync(c => c.Id == residentId);

            if (entity == null)
                return;

            // ============================
            // UPDATE BASIC RESIDENT INFO
            // ============================
            entity.BlockNo = resident.BlockNo;
            entity.UpdatedDate = DateTime.Now;
            entity.Email = resident.Email;
            entity.Name = resident.Name;
            entity.ParkingLotQty = resident.ParkingLotQty;
            entity.HouseNo = resident.HouseNo;
            entity.PhoneNo = resident.PhoneNo;
            entity.LotNo = resident.LotNo;
            entity.Level = resident.Level;
            entity.MaintenenceFeesCost = resident.MaintenenceFeesCost;
            entity.NRIC = resident.NRIC;
            entity.StateId = resident.StateId;
            entity.ParkingLotNos = resident.ParkingLotNos;

            // ============================
            // UPDATE VEHICLE DETAILS
            // ============================
            if (resident.VehicleDetails != null)
            {
                entity.VehicleDetails.Clear();
                foreach (var vehicle in resident.VehicleDetails)
                {
                    entity.VehicleDetails.Add(new VehicleDetails
                    {
                        ResidentId = vehicle.ResidentId,
                        VehicleNo = vehicle.VehicleNo,
                        VehicleTypeId = vehicle.VehicleTypeId,
                        UpdatedDate = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();

            // ============================
            // HANDLE IMAGES
            // ============================

            var existingPhotos = await _context.ResidentPhotos
                .Where(p => p.ResidentId == residentId)
                .ToListAsync();

            string uploadsPath = @"C:\Uploads\ResidentVehicles";

            // 🔹 CASE: If no images sent → delete all
            if (resident.ResidentImages == null || !resident.ResidentImages.Any())
            {
                foreach (var old in existingPhotos)
                {
                    if (!string.IsNullOrEmpty(old.ImagePath) && File.Exists(old.ImagePath))
                    {
                        try { File.Delete(old.ImagePath); } catch { }
                    }
                    _context.ResidentPhotos.Remove(old);
                }

                await _context.SaveChangesAsync();
                return;
            }

            // ============================
            // DELETE REMOVED IMAGES (MISSING IN FRONTEND)
            // ============================

            var incomingFilenames = resident.ResidentImages
                .Where(i => !string.IsNullOrEmpty(i.Preview))
                .Select(i =>
                {
                    if (i.Preview.StartsWith("http"))
                        return Path.GetFileName(new Uri(i.Preview).LocalPath);
                    return null;
                })
                .Where(f => f != null)
                .ToList();

            foreach (var old in existingPhotos)
            {
                var oldFile = Path.GetFileName(old.ImagePath);

                if (!incomingFilenames.Contains(oldFile))
                {
                    if (File.Exists(old.ImagePath))
                    {
                        try { File.Delete(old.ImagePath); } catch { }
                    }

                    _context.ResidentPhotos.Remove(old);
                }
            }

            await _context.SaveChangesAsync();

            // ============================
            // ADD NEW IMAGES
            // ============================

            foreach (var image in resident.ResidentImages)
            {
                if (string.IsNullOrWhiteSpace(image.Preview))
                    continue;

                // NEW BASE64 IMAGE
                if (image.Preview.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    await AddResidentVehicleSelfieAsync(residentId, image.Preview,image.ImageLabel);
                }
                // EXISTING IMAGE URL → ensure DB record exists
                else if (image.Preview.StartsWith("http"))
                {
                    var fileName = Path.GetFileName(new Uri(image.Preview).LocalPath);
                    var fullPath = Path.Combine(uploadsPath, fileName);

                    if (!existingPhotos.Any(p => p.ImagePath.EndsWith(fileName)))
                    {
                        _context.ResidentPhotos.Add(new ResidentPhotos
                        {
                            ResidentId = residentId,
                            Name = fileName,
                            ImagePath = fullPath,
                            ImageLabel=image.ImageLabel,
                            UpdatedDate = DateTime.Now
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }




        public async Task AddResidentVehicleSelfieAsync(int residentId, string imageBase64,string imageLabel)
        {
            if (string.IsNullOrWhiteSpace(imageBase64))
                return;

            string drivePath = @"C:\Uploads\ResidentVehicles";
            if (!Directory.Exists(drivePath))
                Directory.CreateDirectory(drivePath);

            string extension = ".jpg";
            string base64Data = imageBase64;

            if (imageBase64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                var parts = imageBase64.Split(',');
                if (parts.Length == 2)
                {
                    var header = parts[0];
                    base64Data = parts[1];
                    if (header.Contains("png", StringComparison.OrdinalIgnoreCase)) extension = ".png";
                    else if (header.Contains("webp", StringComparison.OrdinalIgnoreCase)) extension = ".webp";
                }
            }

            base64Data = base64Data.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] bytes = Convert.FromBase64String(base64Data);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(drivePath, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            _context.ResidentPhotos.Add(new ResidentPhotos
            {
                ResidentId = residentId,
                Name = fileName,
                ImagePath = filePath,
                UpdatedDate = DateTime.Now,
                ImageLabel= imageLabel
            });

            await _context.SaveChangesAsync();
        }


        public async Task<ResidentDTO> GetResidentsByNRICAsync(string nric, int communityId)
        {
            var residents = await _context.Resident.Where(x => x.NRIC == nric && x.CommunityId == communityId && x.Community.Status == true).FirstOrDefaultAsync();
            return _mapper.Map<ResidentDTO>(residents);
        }
        public async Task<ResidentDTO> GetResidentsNameandContactByAddresses(string roadNo, string blockNo, string level, string houseNo)
        {
            var query = _context.Resident.AsQueryable();

            if (!string.IsNullOrEmpty(roadNo))
                query = query.Where(x => x.RoadNo == roadNo);

            if (!string.IsNullOrEmpty(blockNo))
                query = query.Where(x => x.BlockNo == blockNo);

            if (!string.IsNullOrEmpty(level))
                query = query.Where(x => x.Level == level);

            if (!string.IsNullOrEmpty(houseNo))
                query = query.Where(x => x.HouseNo == houseNo);

            var resident = await query.FirstOrDefaultAsync();

            return _mapper.Map<ResidentDTO>(resident);
        }
        public async Task SendWelcomeEmailAsync(string toEmail, string residentFullName, string tempPassword, string residentPageUrl, string community)
        {
            var fromEmail = "absec.demo@gmail.com";
            var subject = $"Welcome to Community Smart Access - Your Account Has Been Created";
            var body = EmailHelper.GetWelcomeEmailBody(residentFullName, toEmail, tempPassword, residentPageUrl, community);
            using var client = new SmtpClient("smtp.gmail.com") // Replace with your SMTP
            {
                Port = 587,
                Credentials = new NetworkCredential("absec.demo@gmail.com", "qhogkbdwobdoznyx"),
                EnableSsl = true,
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "CSA Team"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            try
            {
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //throw(ex);
            }
        }
        public async Task UpdateResidentVehicleSelfieAsync(int residentId, string imageBase64)
        {
            // ✅ Get existing photos for the resident
            var existingPhotos = await _context.ResidentPhotos
                .Where(x => x.ResidentId == residentId)
                .ToListAsync();

            string drivePath = @"C:\Uploads\ResidentVehicles";
            if (!Directory.Exists(drivePath))
                Directory.CreateDirectory(drivePath);

            // ✅ Remove old images from disk and database
            foreach (var photo in existingPhotos)
            {
                if (!string.IsNullOrEmpty(photo.ImagePath) && File.Exists(photo.ImagePath))
                {
                    try { File.Delete(photo.ImagePath); } catch { /* ignore if file locked */ }
                }
                _context.ResidentPhotos.Remove(photo);
            }

            // ✅ No new image? just save deletions
            if (string.IsNullOrWhiteSpace(imageBase64))
            {
                await _context.SaveChangesAsync();
                return;
            }

            // ✅ Parse Base64 input
            string headerPart = "";
            string base64Data = imageBase64;

            if (imageBase64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                var parts = imageBase64.Split(',');
                if (parts.Length == 2)
                {
                    headerPart = parts[0];
                    base64Data = parts[1];
                }
            }

            string extension = ".jpg"; // default
            if (headerPart.Contains("png", StringComparison.OrdinalIgnoreCase)) extension = ".png";
            else if (headerPart.Contains("jpeg", StringComparison.OrdinalIgnoreCase)) extension = ".jpg";
            else if (headerPart.Contains("webp", StringComparison.OrdinalIgnoreCase)) extension = ".webp";

            // ✅ Clean up Base64 string (remove newlines/spaces)
            base64Data = base64Data.Replace(" ", "")
                                   .Replace("\r", "")
                                   .Replace("\n", "");

            // ✅ Decode image
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid Base64 image format");
            }

            // ✅ Save new file
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(drivePath, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            // ✅ Add the new record
            var newPhoto = new ResidentPhotos
            {
                ResidentId = residentId,
                Name = fileName,
                ImagePath = filePath,
                UpdatedDate = DateTime.Now
            };

            _context.ResidentPhotos.Add(newPhoto);

            // ✅ Save all changes
            await _context.SaveChangesAsync();
        }


        public async Task<string> GetVehicleSelfieByIdAsync(int id)
        {
            string dataUri = string.Empty;
            var Resident = await _context.Resident.Where(x => x.Id == id && x.Community.Status == true).FirstOrDefaultAsync();
            if (Resident != null)
            {
                    var image = Resident.FileName == null ? "" : Resident.FileName;
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\ResidentVehicles")
                                     .Where(f => Path.GetFileName(f)
                                     .Contains(image, StringComparison.OrdinalIgnoreCase))
                                     .ToList();
                    byte[] imageBytes = System.IO.File.ReadAllBytes(Resident.ImagePath);
                    string base64String = Convert.ToBase64String(imageBytes);
                    //base64 = "data:image/" + Path.GetExtension(matchingFiles[0].ToString()) + ";base64," + base64String;
                var bytes = System.IO.File.ReadAllBytes(Resident.ImagePath);
                var base64 = Convert.ToBase64String(bytes);
                dataUri = $"data:image/png;base64,{base64}";
            }
            return dataUri;
        }



        public async Task UpdateResidentProfileAsync(int residentId, ResidentDTO resident)
        {
            var entity = await _context.Resident.Where(x => x.Community != null && x.Community.Status == true)
                               .Include(c => c.VehicleDetails) // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == residentId);
            if (entity != null)
            {
                entity.Email = resident.Email;
                entity.Name = resident.Name;
                entity.PhoneNo = resident.PhoneNo;
                //entity.LotNo = resident.LotNo;
                entity.NRIC = resident.NRIC;
                entity.ContactPerson1 = resident.ContactPerson1;
                entity.ContactPerson2 = resident.ContactPerson2;
                entity.Name = resident.Name;
                var base64Data = resident?.FileName == null ? "" : resident?.FileName.Split(',').Last();
                if (!string.IsNullOrEmpty(base64Data))
                {
                    var imageBytes = Convert.FromBase64String(base64Data);
                    var fileName = $"{Guid.NewGuid()}.png";
                    string drivePath = @"C:\Uploads\ResidentVehicles";
                    // Ensure the directory exists
                    if (!Directory.Exists(drivePath))
                    {
                        Directory.CreateDirectory(drivePath);
                    }
                    var filePath = Path.Combine(drivePath, fileName);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    var fileUrl = $"/uploads/ResidentVehicles/{fileName}";
                    entity.FileName = fileName;
                    entity.ImagePath = fileUrl;
                }
                else
                {
                    entity.FileName = null;
                    entity.ImagePath = null;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateResidentProfileAddressAsync(int residentId, ResidentDTO resident)
        {
            var entity = await _context.Resident.Where(x=>x.Community!=null &&x.Community.Status==true)
                               .Include(c => c.VehicleDetails) // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == residentId);
            if (entity != null)
            {
                entity.Email = resident.Email;
                entity.Name = resident.Name;
                entity.PhoneNo = resident.PhoneNo;
                entity.RoadNo = resident.RoadNo;
                entity.BlockNo = resident.BlockNo;
                entity.Level = resident.Level;
                entity.HouseNo = resident.HouseNo;
              
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteResident(int residentId)
        {
            var Resident = await _context.Resident.Where(x => x.Community != null && x.Community.Status == true).Include(x=>x.VehicleDetails).Where(x => x.Id == residentId).FirstOrDefaultAsync();
            if (Resident != null)
            {
                try
                {
                    var communityDetails = _context.ComplaintDetail.Include(x=>x.ComplaintPhotos)
                                           .Where(cd => cd.ResidentId == residentId);
                    _context.ComplaintDetail.RemoveRange(communityDetails);

                    var eventDetails = _context.EventDetails
                                           .Where(cd => cd.ResidentId == residentId);
                    _context.EventDetails.RemoveRange(eventDetails);

                    var notificationDetails = _context.Notifications
                                           .Where(cd => cd.ResidentId == residentId);
                    _context.Notifications.RemoveRange(notificationDetails);

                    var residentaccessDetails = _context.ResidentAccessHistory
                                          .Where(cd => cd.ResidentId == residentId);
                    _context.ResidentAccessHistory.RemoveRange(residentaccessDetails);

                    var residentFacilityBookingDetails = _context.ResidentFacilityBooking
                                         .Where(cd => cd.ResidentId == residentId);
                    _context.ResidentFacilityBooking.RemoveRange(residentFacilityBookingDetails);

                    var visitorAccessDetails = _context.VisitorAccessDetails
                                        .Where(cd => cd.ResidentId == residentId);
                    _context.VisitorAccessDetails.RemoveRange(visitorAccessDetails);


                    _context.Resident.Remove(Resident); // Delete parent
                    _context.SaveChanges(); // Commit transaction
                }
                catch (Exception ex) {
                    return false;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> SaveResidentVehicleAsync(VehicleModelDTO vehicleDetails)
        {
            if (vehicleDetails == null)
                throw new ArgumentNullException(nameof(vehicleDetails));

            VehicleDetails vehicle;

            if (vehicleDetails.Id == 0)
            {
                // New vehicle
                vehicle = new VehicleDetails
                {
                    VehicleNo = vehicleDetails.VehicleNo,
                    ResidentId = vehicleDetails.ResidentId,
                    VehicleTypeId = vehicleDetails.VehicleTypeId,
                    CreatedDate = DateTime.Now
                };

                _context.VehicleDetails.Add(vehicle);
            }
            else
            {
                // Update existing vehicle
                vehicle = await _context.VehicleDetails.FindAsync(vehicleDetails.Id);

                if (vehicle == null)
                    return false; // Or throw if not expected

                vehicle.VehicleNo = vehicleDetails.VehicleNo;
                vehicle.VehicleTypeId = vehicleDetails.VehicleTypeId;
                vehicle.UpdatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVehicle(int vehicleId)
        {
            var vehicle = await _context.VehicleDetails.Where(x => x.Id == vehicleId).FirstOrDefaultAsync();
            if (vehicle != null)
            {
                try
                {
                    _context.VehicleDetails.Remove(vehicle); // Delete parent
                    _context.SaveChanges(); // Commit transaction
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

    }
}
