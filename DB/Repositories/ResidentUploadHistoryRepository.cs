using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations.Helpers;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories
{
    
    public class ResidentUploadHistoryRepository : RepositoryBase<ResidentUploadHistory, ResidentUploadHistoryDTO>, IResidentUploadHistoryRepository
    {
        public ResidentUploadHistoryRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) :
            base(context, mapper, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<ResidentUploadHistoryDTO>> GetAllResidentUploadHistoryAsync()
        {
                int communityId = await GetUserCommunity();
                var query = from r in _context.ResidentUploadHistory
                            join u in _context.Users on r.UploadedBy equals u.Id
                            where communityId == 0 || r.CommunityId == communityId
                            select new ResidentUploadHistoryDTO
                            {
                                Id = r.Id,
                                FileName = r.FileName,
                                Attachment = r.Attachment,
                                Name = u.Name,
                                UploadDateTime = r.UploadDateTime,
                                CommunityName = r.Community == null ? "" : r.Community.CommunityName
                            };
                var result = await query.OrderByDescending(x => x.Id).ToListAsync();
                return result; // already of type List<ResidentUploadHistoryDTO>, no need to map again
        }
        public async Task<string> UploadData(IFormFile file, string fileName, string attachment,
      string communityId, List<Dictionary<string, object>> rows)
        {
            string Msg = string.Empty;

            try
            {
                foreach (var row in rows)
                {
                    Resident dto = new Resident();
                    dto.Name = row.TryGetValue("Full Name", out var v1) ? v1?.ToString() : "";
                    dto.NRIC = row.TryGetValue("NRIC / Passport No", out var v2) ? v2?.ToString() : "";
                    dto.PhoneNo = row.TryGetValue("Phone No", out var v3) ? v3?.ToString() : "";
                    dto.Email = row.TryGetValue("Email Address", out var v4) ? v4?.ToString() : "";

                    string community = row.TryGetValue("Community ID", out var v5) ? v5?.ToString() : "";
                    dto.CommunityId = _context.Community.Where(x => x.CommunityId == community)
                                                        .Select(x => x.Id).FirstOrDefault();

                    dto.HouseNo = row.TryGetValue("House/Lot No", out var v6) ? v6?.ToString() : "";
                    dto.Level = row.TryGetValue("Level No", out var v7) ? v7?.ToString() : "";
                    dto.BlockNo = row.TryGetValue("Block No", out var v8) ? v8?.ToString() : "";
                    dto.RoadNo = row.TryGetValue("Road No", out var v9) ? v9?.ToString() : "";

                    string parkingLotQty = row.TryGetValue("Parking Lot Qty", out var v10) ? v10?.ToString() : "";
                    dto.ParkingLotQty = string.IsNullOrWhiteSpace(parkingLotQty) ? 0 : Convert.ToInt32(parkingLotQty);

                    dto.LotNo = row.TryGetValue("Parking Lot No", out var v11) ? v11?.ToString() : "";
                    dto.RoleId = 5;
                    dto.Password = EmailHelper.GenerateRandomPassword();
                    dto.IsFirstTimeLogin = true;

                    _context.Resident.Add(dto);
                    _context.SaveChanges();

                    VehicleDetails details = new VehicleDetails();
                    var VehicleType = row.TryGetValue("Vehicle Type", out var v12) ? v12?.ToString() : "";
                    details.VehicleTypeId = _context.VehicleType.FirstOrDefault(x => x.Name == VehicleType)?.Id ?? 0;
                    details.VehicleNo = row.TryGetValue("Vehicle No", out var v13) ? v13?.ToString() : "";
                    details.ResidentId = dto.Id;

                    _context.VehicleDetails.Add(details);
                    _context.SaveChanges();

                    string CName = _context.Community.Where(x => x.CommunityId == community)
                                                     .Select(x => x.CommunityName).FirstOrDefault() ?? "";

                    EmailNotificationQueue queue = new EmailNotificationQueue
                    {
                        RecipientEmail = dto.Email,
                        Subject = "Welcome to Community Smart Access - Your Account Has Been Created",
                        Body = EmailHelper.GetWelcomeEmailBody(dto.Name, dto.Email, dto.Password,
                                "http://103.27.86.226/ResidentApp/login", CName),
                        IsHtml = true,
                        Status = "Pending",
                        RetryCount = 0,
                        MaxRetries = 3,
                        LastAttemptAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        ErrorMessage = ""
                    };

                    _context.EmailNotificationQueue.Add(queue);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw new Exception("Upload Failed");
            }
            finally
            {
                ResidentUploadHistory history = new ResidentUploadHistory
                {
                    Attachment = attachment,
                    FileName = fileName,
                    CommunityId = !string.IsNullOrEmpty(communityId) ? Convert.ToInt32(communityId) : 0,
                    UploadDateTime = DateTime.Now,
                    UploadedBy = Convert.ToInt32(GetCurrentUserId() ?? "0")
                };

                _context.ResidentUploadHistory.Add(history);
                _context.SaveChanges();

                string drivePath = @"C:\Uploads\ResidentExcel";
                if (!Directory.Exists(drivePath))
                    Directory.CreateDirectory(drivePath);

                string extension = Path.GetExtension(attachment);
                var path = Path.Combine(drivePath, $"{fileName}_{history.Id}{extension}");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);  // IMPORTANT
                }

                Msg = "Files uploaded successfully";
            }

            return Msg;
        }



    }
}
