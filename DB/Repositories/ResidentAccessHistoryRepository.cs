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
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class ResidentAccessHistoryRepository : RepositoryBase<ResidentAccessHistory, ResidentAccessHistoryDTO>, IResidentAccessHistoryRepository
    {
        public ResidentAccessHistoryRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) :
            base(context, mapper, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<ResidentAccessHistoryDTO>> GetAllResidentAccessHistoryAsync(int? communityId, bool isCSAAdmin)
        {
            int community = await GetUserCommunity();
            var query = _context.ResidentAccessHistory.OrderByDescending(x => x.Id)
                .Include(c => c.Resident)
                .AsQueryable();

            if (communityId != 0)
            {
                query = query.Where(r => r.Resident.CommunityId == community);
            }

            var residentAccessHistory = await query.ToListAsync();
            var dtoList = _mapper.Map<List<ResidentAccessHistoryDTO>>(residentAccessHistory);

            var currentTime = DateTime.UtcNow;

            foreach (var dto in dtoList)
            {
                if (dto.ValidFrom <= currentTime && dto.ValidTo >= currentTime)
                {
                    dto.Status = "Valid";
                }
                else
                {
                    dto.Status = "Expired";
                }
            }

            return dtoList;
        }

        public async Task<ResidentAccessHistoryDTO> GetResidentAccessHistoryByIdAsync(int? AccessId)
        {
            var query = await _context.ResidentAccessHistory.Where(x => x.Id == AccessId)
                .Include(c => c.Resident).FirstOrDefaultAsync();
            return _mapper.Map<ResidentAccessHistoryDTO>(query);
        }

        public async Task<ResidentAccessHistoryDTO> SaveResidentAccessHistoryAsync(ResidentAccessHistoryDTO resident)
        {
            var ResidentId = _context.Resident.Where(x => x.CommunityId == resident.CommunityId && x.RoadNo == resident.RoadNo
            && x.BlockNo == resident.BlockNo && x.Level == resident.LevelNo && x.HouseNo == resident.HouseNo).Select(x => x.Id).FirstOrDefault();
            resident.ResidentId = ResidentId;
            var entity = _mapper.Map<EFModel.ResidentAccessHistory>(resident);
            _context.ResidentAccessHistory.Add(entity);
            await _context.SaveChangesAsync();
            string Imagedata = string.Empty;
            if(resident.QRImageData.StartsWith("data:image"))
            {
                Imagedata = resident?.QRImageData.Split(",")[1].ToString();
            }
            byte[] imageBytes = Convert.FromBase64String(Imagedata);
            string communityFullName = string.Empty;
            var res = _context.Resident.Where(x => x.Id == ResidentId).FirstOrDefault();
            if (res != null)
            {
                var community = _context.Community.Where(x => x.Id == res.CommunityId).FirstOrDefault();
                if (community != null)
                {
                    communityFullName = community.CommunityId + "-" + community.CommunityName;
                }
                await SendQrEmailAsync(
               toEmail: res.Email,
               residentFullName: resident.Name ?? "Resident",
               imageBytes,
               residentPageUrl: "http://103.27.86.226/ResidentApp/login",
               communityFullName
               );
            }
            return await GetByIdAsync(entity.Id);
        }

        public async Task SendQrEmailAsync(string toEmail, string residentFullName, byte[] imageBytes, string residentPageUrl, string community)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create attachment from MemoryStream
                Attachment imageAttachment = new Attachment(ms, "qrcode.png", "image/png");
                var fromEmail = "absec.demo@gmail.com";
                var subject = $"Welcome to Community Smart Access - Your Account Has Been Created";
                var body = EmailHelper.GetQrEmailBody(residentFullName, toEmail, residentPageUrl, community);
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
                    mailMessage.Attachments.Add(imageAttachment);
                    await client.SendMailAsync(mailMessage);
                    
                }
                catch (Exception ex)
                {
                    //throw(ex);
                }
            }
        }

        public async Task<IEnumerable<ResidentAccessHistoryDTO>> SearchResidentAccessHistoryBySearchParamsAsync(ResidentAccessHistoryDTO searchModel)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            int communityId = await GetUserCommunity();
            var query = _context.ResidentAccessHistory.OrderByDescending(x => x.Id)
                  .Include(c => c.Resident)
                .Include(c => c.Community)              
                .AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(c => c.Resident != null && c.Resident.CommunityId == communityId);
            }
            if (searchModel.CommunityId != 0)
            {
                query = query.Where(c => c.CommunityId == searchModel.CommunityId);
            }
            // Start with base query

            if (!string.IsNullOrEmpty(searchModel.HouseNo))
            {
                query = query.Where(r => r.Resident != null && r.Resident.HouseNo == searchModel.HouseNo);
               
            }
            if (!string.IsNullOrEmpty(searchModel.LevelNo))
            {
                query = query.Where(r => r.Resident != null && r.Resident.Level == searchModel.LevelNo);

            }
            if (!string.IsNullOrEmpty(searchModel.RoadNo))
            {
                query = query.Where(r => r.Resident != null && r.Resident.RoadNo == searchModel.RoadNo);

            }
            if (!string.IsNullOrEmpty(searchModel.BlockNo))
            {
                query = query.Where(r => r.Resident != null && r.Resident.BlockNo == searchModel.BlockNo);

            }

            if (!string.IsNullOrEmpty(searchModel.DateFrom))
            {
                var fromDate = Convert.ToDateTime(searchModel.DateFrom);
                query = query.Where(r => r.EntryTime >= fromDate);
            }
            if (!string.IsNullOrEmpty(searchModel.DateTo))
            {
                var toDate = Convert.ToDateTime(searchModel.DateTo).Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.ExitTime <= toDate);
            }
            var residntAccess = await query.ToListAsync();
            var residntAccessDtos = _mapper.Map<List<ResidentAccessHistoryDTO>>(residntAccess);
            
            return residntAccessDtos;
        }

    }
}
