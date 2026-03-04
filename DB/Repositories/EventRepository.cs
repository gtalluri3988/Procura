using AutoMapper;
using Azure.Core;
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
    public class EventRepository : RepositoryBase<EventDetails, EventDTO>, IEventRepository
    {
        public EventRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<IEnumerable<EventDTO>> GetAllEventsForAdminAsync()
        {
            var residents = await _context.EventDetails.Include(x => x.Resident).Include(x => x.Community).OrderByDescending(x => x.Id).ToListAsync();
            return _mapper.Map<IEnumerable<EventDTO>>(residents);
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsForResidentAsync()
        {
            var residents = await _context.EventDetails.Where(x=>x.ResidentId==Convert.ToInt32(GetCurrentUserId())).Include(x => x.Resident).Include(x => x.Community).OrderByDescending(x => x.EventStart).ToListAsync();
            return _mapper.Map<IEnumerable<EventDTO>>(residents);
        }

        
        public async Task<string> GenerateRunningNo(EventDTO dto)
        {
            var community = _context.Community.Where(x => x.Id == dto.CommunityId).FirstOrDefault();
            var now = DateTime.UtcNow;
            var yy = now.ToString("yy");
            var mm = now.ToString("MM");
            // Get the last event detail ordered by Id descending
            var existing = await _context.EventDetails.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            // Extract the last 3 digits from existing EventRefNo or start from "000"
            string runningNo = "000";
            if (existing != null && !string.IsNullOrEmpty(existing.EventRefNo) && existing.EventRefNo.Length >= 3)
            {
                runningNo = existing.EventRefNo.Substring(existing.EventRefNo.Length - 3, 3);
            }
            int running = int.Parse(runningNo);
            int nextRunning = running + 1;
            string runningStr = nextRunning.ToString("D3");
            // Construct the event code
            var evnCode = $"EVN{community?.CommunityId}{yy}{mm}{runningStr}";
            return evnCode;
        }


        public async Task SendEventQREmail(QRImageModel model)
        {
            string VisitType = string.Empty;
            var resident = await _context.Resident.FindAsync(model.ResidentId);
            if (resident == null || string.IsNullOrWhiteSpace(resident.Email))
                return;

            var imageData = model.ImageData;
            if (string.IsNullOrWhiteSpace(imageData) || !imageData.StartsWith("data:image"))
                return;

            // Extract base64 string
            var base64Data = imageData.Split(',')[1];
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
                // Log or handle bad base64 format
                return;
            }

            var eventDetails = await _context.EventDetails.FindAsync(model.EventId);
            if (eventDetails == null)
                return;
            string communityName=_context.Community?.Where(x=>x.Id== eventDetails.CommunityId).FirstOrDefault()?.CommunityName??"";
            // Determine date range
            string fromDate, toDate;
            fromDate = eventDetails.EventStart.ToString("dd/MM/yyyy hh:mm");
            toDate = eventDetails.EventEnd.ToString("dd/MM/yyyy hh:mm");
            await SendQrEmailAsync(
                toEmail: resident.Email,
                residentFullName: resident.Name ?? "Resident",
                imageBytes,
                fromDate,
                toDate,
                communityName,
                eventDetails.EventDescription
            );
        }


        public async Task SendQrEmailAsync(string toEmail, string residentFullName, byte[] imageBytes, string eventFromDate, string eventToDate, string communityName,string eventDescription)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create attachment from MemoryStream
                Attachment imageAttachment = new Attachment(ms, "qrcode.png", "image/png");
                var fromEmail = "absec.demo@gmail.com";
                var subject =  $"CSA {communityName} | Here is your Event QR";
                var body = EmailHelper.EventEmailQR(toEmail, residentFullName, imageBytes, eventFromDate, eventToDate, communityName, eventDescription);
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

    }
}
