using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations;
using DB.Migrations.Helpers;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
namespace DB.Repositories
{
    public class ComplaintRepository : RepositoryBase<ComplaintDetail, ComplaintDTO>, IComplaintRepository
    {
        public ComplaintRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintsAsync()
        {
            int communityId = await GetUserCommunity();
            var query = _context.ComplaintDetail.Where(x=>x.IsSubmit==true).OrderByDescending(x => x.Id)
                .Include(c => c.ComplaintStatus)
                .Include(c => c.ComplaintType)
                .Include(x => x.Resident)
                .AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(c => c.Resident.CommunityId == communityId);
            }
            var complaints = await query.ToListAsync();
            var complaintsDtos = _mapper.Map<List<ComplaintDTO>>(complaints);
            foreach (var dto in complaintsDtos)
            {
                if (dto.Resident != null)
                {
                    var community = await _context.Community
                        .Where(x => x.Id == dto.Resident.CommunityId)
                        .FirstOrDefaultAsync();
                    dto.CommunityName = community?.CommunityName;
                }
            }
            return complaintsDtos;
        }


        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintsForResidentAsync()
        {
            int communityId = await GetUserCommunity();
            var query = _context.ComplaintDetail.Where(x => x.IsSubmit == true && x.ResidentId==Convert.ToInt32(GetCurrentUserId())).OrderByDescending(x => x.Id)
                .Include(c => c.ComplaintStatus)
                .Include(c => c.ComplaintType)
                .Include(x => x.Resident)
                .AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(c => c.Resident.CommunityId == communityId);
            }
            var complaints = await query.ToListAsync();
            var complaintsDtos = _mapper.Map<List<ComplaintDTO>>(complaints);
            foreach (var dto in complaintsDtos)
            {
                if (dto.Resident != null)
                {
                    var community = await _context.Community
                        .Where(x => x.Id == dto.Resident.CommunityId)
                        .FirstOrDefaultAsync();
                    dto.CommunityName = community?.CommunityName;
                }
            }
            return complaintsDtos;
        }
        public async Task<ComplaintDTO> GetComplaintByComplaintIdAsync(int complaintId)
        {
            var Complaint = await _context.ComplaintDetail.Where(x => x.Id == complaintId).Include(c => c.Resident).Include(x => x.ComplaintPhotos).Include(x => x.ComplaintType).FirstOrDefaultAsync();
            if (Complaint != null)
            {
                foreach (var res in Complaint.ComplaintPhotos)
                {
                    var image = res.ImageGuid == null ? "" : res.ImageGuid;
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                                     .Where(f => Path.GetFileName(f)
                                     .Contains(image, StringComparison.OrdinalIgnoreCase))
                                     .ToList();
                    if (matchingFiles.Any())
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(matchingFiles[0].ToString());
                        string base64String = Convert.ToBase64String(imageBytes);
                        res.Preview = "data:image/" + Path.GetExtension(matchingFiles[0].ToString()) + ";base64," + base64String;
                        res.Name = Path.GetFileName(matchingFiles[0].ToString());
                    }
                }
            }
            return _mapper.Map<ComplaintDTO>(Complaint);
        }
        public async Task UpdateComplaintAsync(int complaintId, ComplaintDTO complaint, List<IFormFile> photos)
        {
            var entity = await _context.ComplaintDetail.Where(x => x.IsSubmit == true)
                               // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == complaintId);
            if (entity != null)
            {
                entity.Description = complaint.Description;
                entity.SecurityRemarks = complaint.SecurityRemarks;
                entity.ComplaintStatusId = complaint.ComplaintStatusId;
                entity.UpdatedDate = DateTime.Now;
                if (complaint.ComplaintPhotos != null)
                {
                    _context.ComplaintPhotos.RemoveRange(_context.ComplaintPhotos.Where(p => p.ComplaintDetailId == complaintId && p.ImageUploadedBy=="Admin"));
                    await _context.SaveChangesAsync();
                    foreach (var item in complaint.ComplaintPhotos)
                    {
                        ComplaintPhotos photo = new ComplaintPhotos();
                        var base64Data = item.Preview == null ? "" : item.Preview.Split(',').Last();
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var fileName = $"{Guid.NewGuid()}.png";
                        string drivePath = @"C:\Uploads\";
                        // Ensure the directory exists
                        if (!Directory.Exists(drivePath))
                        {
                            Directory.CreateDirectory(drivePath);
                        }
                        var filePath = Path.Combine(drivePath, fileName);
                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        var fileUrl = $"/uploads/{fileName}";
                        photo.ImageGuid = fileName;
                        photo.ComplaintDetailId = entity.Id;
                        photo.Name = "";
                        photo.Preview = "";
                        photo.ImageUploadedBy = item.ImageUploadedBy;
                        entity.ComplaintPhotos.Add(photo);
                    }
                }
                if (photos != null)
                {
                    foreach (var file in photos)
                    {
                        if (file.Length > 0)
                        {
                            using var memoryStream = new MemoryStream();
                            await file.CopyToAsync(memoryStream);
                            var fileBytes = memoryStream.ToArray();
                            string base64String = Convert.ToBase64String(fileBytes);
                            ComplaintPhotos photo = new ComplaintPhotos();
                            var imageBytes = Convert.FromBase64String(base64String);
                            var fileName = $"{Guid.NewGuid()}.png";
                            string drivePath = @"C:\Uploads\";
                            // Ensure the directory exists
                            if (!Directory.Exists(drivePath))
                            {
                                Directory.CreateDirectory(drivePath);
                            }
                            var filePath = Path.Combine(drivePath, fileName);
                            System.IO.File.WriteAllBytes(filePath, imageBytes);
                            var fileUrl = $"/uploads/{fileName}";
                            photo.ImageGuid = fileName;
                            photo.ComplaintDetailId = entity.Id;
                            photo.Name = "";
                            photo.Preview = "";
                            entity.ComplaintPhotos.Add(photo);
                        }
                    }
                }
            }
            await SendComplaintStatusUpdateEmailAsync(complaintId,
                complaint
            );
            await _context.SaveChangesAsync();
        }
        public async Task<ComplaintDTO> CreateComplaintAsync(ComplaintDTO dto)
        {
            var entity = _mapper.Map<EFModel.ComplaintDetail>(dto);
            entity.ComplaintPhotos = new List<ComplaintPhotos>();
            _context.ComplaintDetail.Add(entity);
            entity.ComplaintDate = DateTime.Now;
            entity.IsSubmit = true;
            entity.ComplainRefNo = GenerateReferenceNumber();
            if (entity != null && dto.ComplaintPhotos != null)
            {
                foreach (var item in dto.ComplaintPhotos)
                {
                    ComplaintPhotos photo = new ComplaintPhotos();
                    var base64Data = item.Preview == null ? "" : item.Preview.Split(',').Last();
                    var imageBytes = Convert.FromBase64String(base64Data);
                    var fileName = $"{Guid.NewGuid()}.png";
                    string drivePath = @"C:\Uploads\";
                    // Ensure the directory exists
                    if (!Directory.Exists(drivePath))
                    {
                        Directory.CreateDirectory(drivePath);
                    }
                    var filePath = Path.Combine(drivePath, fileName);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    var fileUrl = $"/uploads/{fileName}";
                    photo.ImageGuid = fileName;
                    photo.ComplaintDetailId = entity.Id;
                    photo.Name = "";
                    photo.Preview = "";
                    photo.ImageUploadedBy = item.ImageUploadedBy;
                    //_context.FacilityPhoto.Add(photo);
                    entity.ComplaintPhotos.Add(photo);
                }
            }
            await _context.SaveChangesAsync();
            if (entity != null && entity.Id != 0)
            {
                var comp = _context.ComplaintDetail.Where(x => x.Id == entity.Id).FirstOrDefault();
                if (comp != null)
                {
                    var res = _context.Resident.Where(x => x.Id == comp.ResidentId).FirstOrDefault();
                    if (res != null)
                    {
                        var community = _context.Community.Where(x => x.Id == res.CommunityId).FirstOrDefault();
                        if (community != null)
                        {
                            entity.ComplainRefNo = GenerateRefNo(community.CommunityId, entity.Id);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            await SendComplaintSubmitEmailAsync(
                entity.Id, dto
            );
            return await GetByIdAsync(entity.Id);
        }
        public static string GenerateRefNo(string communityId, int runningNo)
        {
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            string paddedRunningNo = runningNo.ToString("D4"); // e.g., 0001
            return $"CMP{communityId}{year}{month}{paddedRunningNo}";
        }
        public async Task<IEnumerable<ComplaintDTO>> GetAllComplaintsByCommunityAsync(int communityId)
        {
            var complaints = await _context.ComplaintDetail.Where(x => x.IsSubmit == true).Include(c => c.ComplaintStatus)
                .Include(c => c.ComplaintType).Include(x => x.Resident).ToListAsync();
            complaints = complaints
    .Where(x => x.Resident != null && x.Resident.CommunityId == communityId)
    .ToList();
            var complaintsDtos = _mapper.Map<List<ComplaintDTO>>(complaints);
            // Attach PaymentStatus manually
            foreach (var dto in complaintsDtos)
            {
                if (dto.Resident != null)
                {
                    var CommunityName = _context.Community.Where(x => x.Id == dto.Resident.CommunityId).FirstOrDefault();
                    dto.CommunityName = CommunityName?.CommunityName;
                }
            }
            return _mapper.Map<IEnumerable<ComplaintDTO>>(complaintsDtos);
        }
        public string GenerateReferenceNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // e.g., "20240525153045"
            return $"REF-{timestamp}";
        }
        public async Task SendComplaintEmailAsync(ComplaintDTO dto)
        {
            var complaint = _context.ComplaintDetail.Where(x => x.ComplainRefNo == dto.ComplainRefNo).Include(x => x.ComplaintType).FirstOrDefault();
            var fromEmail = "absec.demo@gmail.com";
            var subject = $"Welcome to Community Smart Access - Your Account Has Been Created";
            var body = complaint != null ? EmailHelper.ComplaintEmailBody(complaint) : string.Empty;
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
                mailMessage.To.Add(dto.Resident?.Email);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //throw(ex);
            }
        }
        public async Task SendComplaintSubmitEmailAsync(int complaintId, ComplaintDTO dto)
        {
            string communityName = string.Empty;
            var complaint = _context.ComplaintDetail.Where(x => x.Id == complaintId).Include(x => x.ComplaintType).FirstOrDefault();
            if (complaint != null)
            {
                communityName = (from r in _context.Resident
                                 join c in _context.Community on r.CommunityId equals c.Id
                                 where r.Id == complaint.ResidentId
                                 select c.CommunityName).FirstOrDefault() ?? string.Empty;
            }
            var fromEmail = "absec.demo@gmail.com";
            var subject = $"CSA {communityName} | We've received your complaint";
            var body = complaint != null ? EmailHelper.ComplaintEmailSubmissionBody(complaint,communityName) : string.Empty;
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
                string folderPath = @"C:\Uploads";
                foreach (var fileName in complaint?.ComplaintPhotos)
                {
                    string fullPath = Path.Combine(folderPath, fileName.ImageGuid);
                    if (File.Exists(fullPath))
                    {
                        Attachment attachment = new Attachment(fullPath, MediaTypeNames.Application.Octet);
                        mailMessage.Attachments.Add(attachment);
                    }
                }
                mailMessage.To.Add(complaint.Resident?.Email);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //throw(ex);
            }
        }
        public async Task SendComplaintStatusUpdateEmailAsync(int complaintId,ComplaintDTO dto)
        {
            string communityName = string.Empty;
            var complaint = _context.ComplaintDetail.Where(x => x.Id == complaintId).Include(x => x.ComplaintType).Include(x=>x.ComplaintStatus).Include(x=>x.Resident).FirstOrDefault();
            if (complaint != null)
            {
                communityName = (from r in _context.Resident
                                 join c in _context.Community on r.CommunityId equals c.Id
                                 where r.Id == complaint.ResidentId
                                 select c.CommunityName).FirstOrDefault() ?? string.Empty;
            }
            var fromEmail = "absec.demo@gmail.com";
            var subject = $"CSA {communityName} | Update regarding your complaint";
            var body = complaint != null ? EmailHelper.ComplaintStatusUpdateEmailBody(complaint, communityName) : string.Empty;
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
                string folderPath = @"C:\Uploads";
                foreach (var fileName in complaint?.ComplaintPhotos)
                {
                    string fullPath = Path.Combine(folderPath, fileName.ImageGuid);
                    if (File.Exists(fullPath))
                    {
                        Attachment attachment = new Attachment(fullPath, MediaTypeNames.Application.Octet);
                        mailMessage.Attachments.Add(attachment);
                    }
                }
                mailMessage.To.Add(dto.Resident?.Email);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //throw(ex);
            }
        }

        public async Task<IEnumerable<ComplaintDTO>> SearchComplaintBySearchParamsAsync(ComplaintDTO searchModel)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            int communityId = await GetUserCommunity();
            var query = _context.ComplaintDetail.Where(x => x.IsSubmit == true).OrderByDescending(x => x.Id)
                  .Include(c => c.ComplaintStatus)
                .Include(c => c.ComplaintType)
                .Include(x => x.Resident)
                .AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(c => c.Resident.CommunityId == communityId);
            }
            if(searchModel.CommunityId!=0)
            {
                query = query.Where(c => c.Resident.CommunityId == searchModel.CommunityId);
            }
            // Start with base query
            if (searchModel.ComplaintStatusId!=0)
                query = query.Where(r => r.ComplaintStatusId == searchModel.ComplaintStatusId);
            if (searchModel.ComplaintTypeId != 0)
                query = query.Where(r => r.ComplaintTypeId == searchModel.ComplaintTypeId);
            if (!string.IsNullOrEmpty(searchModel.DateFrom))
            {
                var fromDate = Convert.ToDateTime(searchModel.DateFrom);
                query = query.Where(r => r.ComplaintDate >= fromDate);
            }
            if (!string.IsNullOrEmpty(searchModel.DateTo))
            {
                var toDate = Convert.ToDateTime(searchModel.DateTo).Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.ComplaintDate <= toDate);
            }
            var complaints = await query.ToListAsync();
            var complaintsDtos = _mapper.Map<List<ComplaintDTO>>(complaints);
            foreach (var dto in complaintsDtos)
            {
                if (dto.Resident != null)
                {
                    var community = await _context.Community
                        .Where(x => x.Id == dto.Resident.CommunityId)
                        .FirstOrDefaultAsync();
                    dto.CommunityName = community?.CommunityName;
                }
            }
            return complaintsDtos;
        }

        public async Task SubmitComplaintAsync(int complaintId)
        {
            string communityName = string.Empty;
            var complaint =await _context.ComplaintDetail.Where(x => x.Id == complaintId).FirstOrDefaultAsync();
            if (complaint != null)
            {
                complaint.IsSubmit= true;
                await _context.SaveChangesAsync();
            }
        }
     }
}
