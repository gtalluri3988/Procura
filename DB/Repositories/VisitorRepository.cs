using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations.Helpers;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace DB.Repositories
{
    public class VisitorRepository : RepositoryBase<VisitorAccessDetails, VisitorAccessDetailsDTO>, IVisitorRepository
    {
        public VisitorRepository(CSADbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }


        public async Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync()
        {
            int communityId = await GetUserCommunity();
            int userId = Convert.ToInt32(GetCurrentUserId());

            var role = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            // -------------------------------
            // Base query
            // -------------------------------
            var query = _context.VisitorAccessDetails
                .Include(v => v.VisitorAccessType)
                .Include(v => v.Community)
                .Include(v => v.Resident)
                .Include(v => v.VisitorDetailsTransactions)
                .AsQueryable();

            // -------------------------------
            // Filters
            // -------------------------------
            if (role != 2) // Resident
            {
                query = query.Where(x =>
                    x.ResidentId == userId &&
                    x.IsAddByResident == true);
            }
            else if (communityId != 0) // Admin with community
            {
                query = query.Where(x => x.CommunityId == communityId);
            }

            // -------------------------------
            // Lookups
            // -------------------------------
            var residentNames = await _context.Resident
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var userNames = await _context.Users
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            // -------------------------------
            // EF-SAFE QUERY (no formatting)
            // -------------------------------
            var rawResult = await query
     .SelectMany(
         v => v.VisitorDetailsTransactions,
         (v, t) => new
         {
             v.Id,
             v.VisitorName,
             CommunityName = v.Community != null ? v.Community.CommunityName : null,
             VisitTypeName = v.VisitorAccessType != null ? v.VisitorAccessType.Name : null,
             v.VisitPurpose,

             HouseNo = v.Resident != null ? v.Resident.HouseNo : null,
             BlockNo = v.Resident != null ? v.Resident.BlockNo : null,
             LevelNo = v.Resident != null ? v.Resident.Level : null,
             RoadNo = v.Resident != null ? v.Resident.RoadNo : null,

             v.VehicleNo,
             ResidentName = v.Resident != null ? v.Resident.Name : null,

             EntryTime = t.EntryTime,
             ExitTime = t.ExitTime,
             CreatedDate = t.CreatedDate,

             v.IsAddByAdmin,
             v.VisitDate,
             v.RegisterBy,
             v.ResidentId,
             v.CreatedBy,
             TransactionVisitorId = t.VisitorId
         })
    .OrderByDescending(x => x.TransactionVisitorId)          // ✅ VisitorId
    .ThenByDescending(x => x.EntryTime)             // ✅ latest first
    .ToListAsync();
            // -------------------------------
            // IN-MEMORY PROJECTION (SAFE)
            // -------------------------------
            var result = rawResult.Select(x => new VisitorAccessModel
            {
                VisitorId = x.Id,
                VisitorName = x.VisitorName,
                CommunityName = x.CommunityName,
                VisitTypeName = x.VisitTypeName,
                VisitPurpose = x.VisitPurpose,

                HouseNo = x.HouseNo,
                BlockNo = x.BlockNo,
                LevelNo = x.LevelNo,
                RoadNo = x.RoadNo,

                VehicleNo = x.VehicleNo,
                ResidentName = x.ResidentName,

                EntryTime = x.EntryTime,
                ExitTime = x.ExitTime,
                CreatedDate = x.CreatedDate,

                IsAddByAdmin = x.IsAddByAdmin,

                VisitDate = x.VisitDate.HasValue
                    ? x.VisitDate.Value.ToString("dd/MM/yyyy")
                    : null,

                IsResidentRegister = x.RegisterBy == true ? "Yes" : "No",

                RegisterBy = (x.RegisterBy ?? false)
                    ? (x.ResidentId != null &&
                       residentNames.TryGetValue(x.ResidentId.Value, out var rname)
                        ? rname
                        : null)
                    : (int.TryParse(x.CreatedBy, out var uid) &&
                       userNames.TryGetValue(uid, out var uname)
                        ? uname
                        : null)

            }).ToList();

            return result;
        }



        public async Task<IEnumerable<VisitorAccessModel>> GetAllCommunityAdminVisitorsAsync()
        {
            int communityId = await GetUserCommunity();
            int userId = Convert.ToInt32(GetCurrentUserId());

            var roleId = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            // -------------------------------
            // BASE QUERY
            // -------------------------------
            var query = _context.VisitorAccessDetails
                .Include(v => v.VisitorAccessType)
                .Include(v => v.Community)
                .Include(v => v.Resident)
                .Include(v => v.VisitorDetailsTransactions)
                .AsQueryable();

            // -------------------------------
            // ROLE / COMMUNITY FILTER
            // -------------------------------
            if (roleId == 2) // Admin
            {
                if (communityId != 0)
                    query = query.Where(x => x.CommunityId == communityId);
                // else super admin → no filter
            }
            else
            {
                // Non-admins restricted to community
                query = query.Where(x => x.CommunityId == communityId);
            }

            // -------------------------------
            // LOOKUPS
            // -------------------------------
            var residentNames = await _context.Resident
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var userNames = await _context.Users
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            // -------------------------------
            // EF-SAFE QUERY (FLATTEN TRANSACTIONS)
            // -------------------------------
            var rawResult = await query
      .SelectMany(
          v => v.VisitorDetailsTransactions,
          (v, t) => new
          {
              v.Id,
              v.VisitorName,
              CommunityName = v.Community != null ? v.Community.CommunityName : null,
              VisitTypeName = v.VisitorAccessType != null ? v.VisitorAccessType.Name : null,
              v.VisitPurpose,

              HouseNo = v.Resident != null ? v.Resident.HouseNo : null,
              BlockNo = v.Resident != null ? v.Resident.BlockNo : null,
              LevelNo = v.Resident != null ? v.Resident.Level : null,
              RoadNo = v.Resident != null ? v.Resident.RoadNo : null,

              v.VehicleNo,
              ResidentName = v.Resident != null ? v.Resident.Name : null,

              EntryTime = t.EntryTime,
              ExitTime = t.ExitTime,
              CreatedDate = t.CreatedDate,

              v.IsAddByAdmin,
              v.VisitDate,
              v.RegisterBy,
              v.ResidentId,
              v.CreatedBy,
              TransactionVisitorId = t.VisitorId
          })
     .OrderByDescending(x => x.TransactionVisitorId)          // ✅ VisitorId
     .ThenByDescending(x => x.EntryTime)             // ✅ latest first
     .ToListAsync();

            // -------------------------------
            // IN-MEMORY PROJECTION
            // -------------------------------
            var result = rawResult.Select(x => new VisitorAccessModel
            {
                VisitorId = x.Id,
                VisitorName = x.VisitorName,
                CommunityName = x.CommunityName,
                VisitTypeName = x.VisitTypeName,
                VisitPurpose = x.VisitPurpose,

                HouseNo = x.HouseNo,
                BlockNo = x.BlockNo,
                LevelNo = x.LevelNo,
                RoadNo = x.RoadNo,

                VehicleNo = x.VehicleNo,
                ResidentName = x.ResidentName,

                EntryTime = x.EntryTime,
                ExitTime = x.ExitTime,
                CreatedDate = x.CreatedDate,

                IsAddByAdmin = x.IsAddByAdmin,

                VisitDate = x.VisitDate.HasValue
                    ? x.VisitDate.Value.ToString("dd/MM/yyyy")
                    : null,

                IsResidentRegister = x.RegisterBy == true ? "Yes" : "No",

                RegisterBy = (x.RegisterBy ?? false)
                    ? (x.ResidentId != null &&
                       residentNames.TryGetValue(x.ResidentId.Value, out var rname)
                        ? rname
                        : null)
                    : (int.TryParse(x.CreatedBy, out var uid) &&
                       userNames.TryGetValue(uid, out var uname)
                        ? uname
                        : null)

            }).ToList();

            return result;
        }


        public async Task<IEnumerable<VisitorAccessModel>> GetAllVisitorsAsync(int communityId)
        {
          
            // Fetch main entity list
            var visitorAccessDetails = await _context.VisitorAccessDetails.Where(x => x.ResidentId == Convert.ToInt32(GetCurrentUserId()))
                .Include(c => c.VisitorAccessType)
                .Include(x => x.Community).OrderByDescending(x => x.Id)
                .ToListAsync();
            var Role = await _context.Users.Where(x => x.Id == Convert.ToInt32(GetCurrentUserId())).FirstOrDefaultAsync();
            if (Role?.RoleId == 2)
            {
                if (communityId == 0)
                {
                    visitorAccessDetails = await _context.VisitorAccessDetails
                                     .Include(c => c.VisitorAccessType)
                                     .Include(x => x.Community).OrderByDescending(x => x.Id)
                                     .ToListAsync();
                }
                else
                {
                    visitorAccessDetails = await _context.VisitorAccessDetails.Where(x => x.CommunityId == communityId)
                                     .Include(c => c.VisitorAccessType)
                                     .Include(x => x.Community).OrderByDescending(x => x.Id)
                                     .ToListAsync();
                }
            }



            // Preload related lookup data into dictionaries
            var residentNames = await _context.Resident
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var userNames = await _context.Users
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            // Project to model safely using in-memory dictionaries
            var result = visitorAccessDetails.Select(d => new VisitorAccessModel
            {
                VisitorId = d.Id,
                VisitorName = d.VisitorName,
                CommunityName = d.Community?.CommunityName,
                VisitTypeName = d.VisitorAccessType?.Name,
                VisitPurpose = d.VisitPurpose,
                HouseNo = d.Resident?.HouseNo,
                BlockNo = d.Resident?.BlockNo,
                LevelNo = d.Resident?.Level,
                RoadNo = d.Resident?.RoadNo,
                VehicleNo = d.VehicleNo,
                ResidentName = d.Resident?.Name,
                IsAddByAdmin = d.IsAddByAdmin,
                EntryTime = d.EntryTime,
                ExitTime = d.ExitTime,
                CreatedDate = d.CreatedDate,
                VisitDate = d.VisitDate?.ToString("dd/MM/yyyy"),
                IsResidentRegister = d.RegisterBy == true ? "Yes" : "No",
                RegisterBy = (d.RegisterBy ?? false)
                    ? (d.ResidentId != null && residentNames.TryGetValue(d.ResidentId.Value, out var residentName)
                        ? residentName
                        : null)
                    : (int.TryParse(d.CreatedBy, out var userId) && userNames.TryGetValue(userId, out var userName)
                        ? userName
                        : null)
            }).ToList();

            return result;
        }


        public async Task<IEnumerable<VisitorAccessDetailsDTO>> GetAllVisitorsByCommunityAsync(int communityId)
        {
            var Visitors = await _context.VisitorAccessDetails.Where(x=>x.CommunityId== communityId).Include(c => c.VisitorAccessType).Include(x => x.Community).Include(x=>x.Resident).OrderByDescending(x => x.Id).ToListAsync();
            return _mapper.Map<IEnumerable<VisitorAccessDetailsDTO>>(Visitors);
        }

        public async Task<VisitorAccessDetailsDTO> GetVisitorsByCommunityIdResidentIdAsync(int visitorId)
        {
            var Visitors = await _context.VisitorAccessDetails.Where(x => x.Id == visitorId)
                .Include(c => c.VisitorAccessType).Include(x => x.Community).ThenInclude(c=>c.VisitorParkingCharges).ThenInclude(x=>x.ChargeType).Include(x => x.Resident).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return _mapper.Map<VisitorAccessDetailsDTO>(Visitors);
        }
        public async Task<IEnumerable<VisitorAccessModel>> SearchVisitorsByCommunityIdAsync(VisitorAccessDetailsDTO searchModel)
        {
            int communityId = await GetUserCommunity();

            var query = _context.VisitorAccessDetails
                .Include(c => c.VisitorAccessType)
                .Include(x => x.Community)
                .AsQueryable();

            if (communityId != 0)
            {
                query = query.Where(v => v.CommunityId == communityId);
            }

            // Apply filters based on provided values
            if (!string.IsNullOrEmpty(searchModel.DateFrom))
            {
                var fromDate =Convert.ToDateTime(searchModel.DateFrom);
                query = query.Where(r => r.EntryTime >= fromDate || r.VisitDate>=fromDate);
            }

            if (!string.IsNullOrEmpty(searchModel.DateTo))
            {
                // Include entire day by setting time to 23:59:59
                var toDate = Convert.ToDateTime(searchModel.DateTo);
                query = query.Where(r => r.ExitTime <= toDate || r.VisitDate <= toDate);
            }

            if ( searchModel.VisitorAccessTypeId != 0)
            {
                query = query.Where(r => r.VisitorAccessTypeId == searchModel.VisitorAccessTypeId);
            }
            var visitors = await query.ToListAsync();
            if (!string.IsNullOrWhiteSpace(searchModel.Status))
            {
                visitors = visitors.Where(v =>
                {
                    var computedStatus = v.EntryTime != null && v.ExitTime != null ? "Exit" : "Entry";
                    return computedStatus.Equals(searchModel.Status, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }
            var visitorAccessDetails = await _context.VisitorAccessDetails
                .Include(c => c.VisitorAccessType)
                .Include(x => x.Community).OrderByDescending(x => x.Id)

                .ToListAsync();

            // Preload related lookup data into dictionaries
            var residentNames = await _context.Resident
                .ToDictionaryAsync(r => r.Id, r => r.Name);

            var userNames = await _context.Users
                .ToDictionaryAsync(u => u.Id, u => u.Name);
            var result = visitors.Select(d => new VisitorAccessModel
            {
                VisitorName = d.VisitorName,
                CommunityName = d.Community?.CommunityName,
                VisitTypeName = d.VisitorAccessType?.Name,
                VisitPurpose = d.VisitPurpose,
                HouseNo = d.Resident?.HouseNo,
                BlockNo = d.Resident?.BlockNo,
                LevelNo = d.Resident?.Level,
                RoadNo = d.Resident?.RoadNo,
                VehicleNo = d.VehicleNo,
                ResidentName = d.Resident?.Name,
                EntryTime = d.EntryTime,
                ExitTime = d.ExitTime,
                CreatedDate = d.CreatedDate,
                VisitDate = d.VisitDate?.ToString("dd/MM/yyyy"),
                IsResidentRegister = d.RegisterBy == true ? "Yes" : "No",
                RegisterBy = (d.RegisterBy ?? false)
                    ? (d.ResidentId != null && residentNames.TryGetValue(d.ResidentId.Value, out var residentName)
                        ? residentName
                        : null)
                    : (int.TryParse(d.CreatedBy, out var userId) && userNames.TryGetValue(userId, out var userName)
                        ? userName
                        : null)
            }).ToList();

            var visitorDtos = _mapper.Map<List<VisitorAccessDetailsDTO>>(visitors);

            return result;
        }

        public async Task<VisitorAccessDetailsDTO> SaveVisitorDetailsAsync(VisitorAccessDetailsDTO resident)
        {
            var ResidentId = _context.Resident.Where(x => x.CommunityId == resident.CommunityId && x.RoadNo == resident.RoadNo
            && x.BlockNo == resident.BlockNo && x.Level == resident.LevelNo && x.HouseNo == resident.HouseNo).Select(x => x.Id).FirstOrDefault();
            var residentDetails = _context.Resident.Where(x => x.Id == ResidentId).FirstOrDefault();
            var entity = _mapper.Map<EFModel.VisitorAccessDetails>(resident);
            if (residentDetails != null)
            {
                entity.ResidentId = resident.ResidentId==0? ResidentId: resident.ResidentId;
                entity.RegisterBy = false;
                entity.RoadNo= residentDetails.RoadNo; 
                entity.BlockNo= residentDetails.BlockNo;
                entity.LevelNo = residentDetails.Level;
                entity.HouseNo= residentDetails.HouseNo;
                entity.VisitorAccessTypeId = resident.VisitorAccessTypeId;
                entity.VisitDate = DateTime.Now;
                _context.VisitorAccessDetails.Add(entity);
                try
                {
                    await _context.SaveChangesAsync();


                    // -------------------------------
                    // CREATE FIRST TRANSACTION (ENTRY)
                    // -------------------------------
                    var transaction = new VisitorDetailsTransaction
                    {
                        VisitorId = entity.Id,          // 🔑 FK
                        EntryTime = null,       // ENTRY
                        ExitTime = null,
                        CreatedDate = DateTime.Now
                    };

                    _context.VisitorDetailsTransactions.Add(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
            }
            return await GetByIdAsync(entity.Id);

        }

        public async Task<VisitorAccessDetailsDTO> SaveVisitorMobileDetailsAsync(VisitorAccessDetailsDTO resident)
        {
            if (resident?.DateFrom != null && resident?.DateTo != null && resident?.VisitorAccessTypeId != null 
                && CheckMaxQrAllowed(resident.DateFrom, resident.DateTo, resident.VisitorAccessTypeId, resident.ResidentId))
            {
                throw new Exception("Frequent access visitors are limited to 2 active QR codes. Wait for an existing QR code to expire before generating a new one.");
            }

            var residentDetails = _context.Resident.Where(x => x.Id == resident.ResidentId).FirstOrDefault();
            var entity = _mapper.Map<EFModel.VisitorAccessDetails>(resident);
            if (residentDetails != null)
            {
                entity.ResidentId = resident.ResidentId;
                entity.RegisterBy = true;
                entity.RoadNo = residentDetails.RoadNo;
                entity.BlockNo = residentDetails.BlockNo;
                entity.VehicleNo = resident.VehicleNo?.Replace(" ", "");
                entity.LevelNo = residentDetails.Level;
                entity.HouseNo = residentDetails.HouseNo;
                entity.EntryTime = resident.DateFrom!=null? Convert.ToDateTime(resident.DateFrom):null;
                entity.ExitTime = resident.DateTo != null ? Convert.ToDateTime(resident.DateTo) : null;
                entity.IsAddByResident = true;
                _context.VisitorAccessDetails.Add(entity);
                await _context.SaveChangesAsync();


                var transaction = new VisitorDetailsTransaction
                {
                    VisitorId = entity.Id,          // 🔑 FK
                    EntryTime = null,       // ENTRY
                    ExitTime = null,
                    CreatedDate = DateTime.Now
                };
                _context.VisitorDetailsTransactions.Add(transaction);
                await _context.SaveChangesAsync();

            }
            return await GetByIdAsync(entity.Id);

        }
        private bool CheckMaxQrAllowed(string DateFrom,string DateTo, int VisitorAccessTypeId,int residentId)
        {
            DateTime EntryDate = Convert.ToDateTime(DateFrom);
            DateTime ExitDate = Convert.ToDateTime(DateTo);
            var VisitDetails = _context.VisitorAccessDetails.Where(x => x.VisitorAccessTypeId == VisitorAccessTypeId && x.ResidentId==residentId 
                                && x.EntryTime >= EntryDate && x.ExitTime <= ExitDate).ToList();
            if(VisitDetails.Count>=2)
            {
                return true;
            }
            return false;
        }


        public async Task SendVisitorQREmail(QRImageModel model)
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

            var visitor = await _context.VisitorAccessDetails.FindAsync(model.VisitorId);
            if (visitor == null)
                return;
            string communityName = _context.Community?.Where(x => x.Id == visitor.CommunityId).FirstOrDefault()?.CommunityName ?? "";
            // Determine date range
            string fromDate, toDate;
            VisitType = _context.VisitorAccessType.Where(x => x.Id == visitor.VisitorAccessTypeId).Select(x => x.Name).FirstOrDefault()??"";
            if (visitor.VisitorAccessTypeId == 30 && visitor.EntryTime.HasValue && visitor.ExitTime.HasValue)
            {
                fromDate = visitor.EntryTime.Value.ToString("dd/MM/yyyy");
                toDate = visitor.ExitTime.Value.ToString("dd/MM/yyyy");
            }
            else if (visitor.VisitDate.HasValue)
            {
                fromDate = toDate = visitor.VisitDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                // Invalid date data
                return;
            }

            await SendQrEmailAsync(
                toEmail: resident.Email,
                residentFullName: resident.Name ?? "Resident",
                imageBytes,
                communityName,
                fromDate,
                toDate,
                VisitType
            );
        }


        public async Task SendQrEmailAsync(string toEmail, string residentFullName, byte[] imageBytes,string communityName, string visitFromDate, string visitToDate,string visitType)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create attachment from MemoryStream
                Attachment imageAttachment = new Attachment(ms, "qrcode.png", "image/png");
                var fromEmail = "absec.demo@gmail.com";
                var subject = $"CSA {communityName} | Here is your visitor QR";
                var body = EmailHelper.VisitorEmailQR(toEmail, residentFullName, imageBytes, visitFromDate, visitToDate, visitType);
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

        public static double CalculateParkingFee(DateTime entry, DateTime exit)
        {
            // Rates
            const double firstHourRate = 1.0;
            const double subsequentHourRate = 0.5;
            const int freeHours = 4;
            const double maxDailyRate = 5.0;
            const double overnightRate = 10.0;

            // Calculate total hours
            TimeSpan duration = exit - entry;
            double totalHours = duration.TotalHours;
            if (totalHours < 0)
                throw new ArgumentException("Exit time must be after entry time");

            // Overnight condition (exit between 12 AM and 6 AM next day)
            if (exit.Hour >= 0 && exit.Hour < 6)
            {
                return overnightRate;
            }

            // Calculate normal charges
            double payableHours = Math.Max(0, Math.Ceiling(totalHours - freeHours));
            double amount = 0.0;

            if (payableHours > 0)
            {
                amount = firstHourRate; // 1st chargeable hour
                if (payableHours > 1)
                {
                    amount += (payableHours - 1) * subsequentHourRate;
                }
            }

            // Apply max daily cap
            if (amount > maxDailyRate)
                amount = maxDailyRate;

            return amount;
        }



        //public static double CalculateParkingFee(DateTime entry, DateTime exit)
        //{
        //    // ===== Rate Definitions =====
        //    const int freeHours = 4;
        //    const double firstHourRate = 1.0;
        //    const double subsequentHourRate = 0.5;
        //    const double maxDailyRate = 5.0;
        //    const double overnightRate = 10.0;

        //    if (exit <= entry)
        //        throw new ArgumentException("Exit time must be after entry time");

        //    double totalAmount = 0.0;
        //    DateTime currentDayStart = entry;

        //    // ===== Handle multiple days =====
        //    while (currentDayStart.Date < exit.Date)
        //    {
        //        DateTime nextMidnight = currentDayStart.Date.AddDays(1);
        //        TimeSpan dayDuration = nextMidnight - currentDayStart;

        //        // Calculate charge for that day (till midnight)
        //        double hours = dayDuration.TotalHours;
        //        double amount = CalculateDailyCharge(hours, freeHours, firstHourRate, subsequentHourRate, maxDailyRate);

        //        totalAmount += Math.Min(amount, maxDailyRate);
        //        currentDayStart = nextMidnight;
        //    }

        //    // ===== Handle last day =====
        //    TimeSpan lastDuration = exit - currentDayStart;

        //    // If exit falls under overnight (12 AM–6 AM)
        //    if (exit.Hour >= 0 && exit.Hour < 6)
        //    {
        //        totalAmount += overnightRate;
        //    }
        //    else
        //    {
        //        double hours = lastDuration.TotalHours;
        //        double amount = CalculateDailyCharge(hours, freeHours, firstHourRate, subsequentHourRate, maxDailyRate);
        //        totalAmount += Math.Min(amount, maxDailyRate);
        //    }

        //    return totalAmount;
        //}

        //private static double CalculateDailyCharge(double hours, int freeHours, double firstHourRate, double subsRate, double maxDailyRate)
        //{
        //    double payableHours = Math.Max(0, Math.Ceiling(hours - freeHours));
        //    double amount = 0.0;

        //    if (payableHours > 0)
        //    {
        //        amount = firstHourRate;
        //        if (payableHours > 1)
        //            amount += (payableHours - 1) * subsRate;
        //    }

        //    return Math.Min(amount, maxDailyRate);
        //}

        public async Task<VisitorAccessDetailsDTO> GetVisitorByVehicleNoAsync(string vehicleNo)
        {
            var visitorId = await _context.VisitorAccessDetails.Where(x => x.VehicleNo == vehicleNo).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            var Visitors = await _context.VisitorAccessDetails.Where(x => x.Id == visitorId.Id)
                .Include(c => c.VisitorAccessType).Include(x => x.Community).ThenInclude(c => c.VisitorParkingCharges)
                .ThenInclude(x => x.ChargeType).Include(x => x.Resident).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            return _mapper.Map<VisitorAccessDetailsDTO>(Visitors);
        }

    }


}
