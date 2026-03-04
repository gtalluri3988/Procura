using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{

    public class ReportRepository : IReportRepository
    {
        private readonly CSADbContext _context;

        public ReportRepository(CSADbContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlyCollectionReportDto>> GetCollectionSummaryReportAsync(int? communityId)
        {
            try
            {

                var data = await _context.MaintanencePaymentStatus
                    .Where(x => x.PaymentStatus == "Paid")
                    .Select(x => new
                    {
                        x.MaintananceYear,
                        x.MaintananceMonth,
                        x.FeeType,
                        x.Amount
                    })
                    .ToListAsync(); // bring data to memory

                var report = data
                    .GroupBy(x => new { x.MaintananceYear, x.MaintananceMonth })
                    .Select(g => new MonthlyCollectionReportDto
                    {
                        Month = new DateTime(g.Key.MaintananceYear.Value, g.Key.MaintananceMonth.Value, 1)
                                    .ToString("MMM-yy"),

                        MaintenanceCollected = g
                            .Where(x => x.FeeType == "Maintenance")
                            .Sum(x => decimal.TryParse(x.Amount, out var v) ? v : 0),

                        SinkingFundCollected = g
                            .Where(x => x.FeeType == "SinkingFund")
                            .Sum(x => decimal.TryParse(x.Amount, out var v) ? v : 0),

                        OtherFees = g
                            .Where(x => x.FeeType == "Other")
                            .Sum(x => decimal.TryParse(x.Amount, out var v) ? v : 0)
                    })
                    .OrderBy(x => DateTime.ParseExact(x.Month, "MMM-yy", null))
                    .ToList();
                var rep = report;
                return report;
            }
            catch(Exception ex)
            {

            }
            return null;
        }


        public async Task<List<FacilityUsageReportDto>> GetFacilityUsageReportAsync()
        {
            try
            {
                // 1️⃣ Load bookings + payments
                var data = await (
                    from b in _context.ResidentFacilityBooking
                        .Include(x => x.Facility)
                    join p in _context.FacilityPaymentStatus
                        on b.Id equals p.FacilityBookingId into pay
                    from p in pay.DefaultIfEmpty() // LEFT JOIN
                    select new
                    {
                        Booking = b,
                        PaymentStatus = p != null ? p.PaymentStatus : "Pending",
                        EffectiveDate = b.StartDate ?? b.CreatedDate
                    }
                ).ToListAsync();

                // 2️⃣ Group & calculate report
                var report = data
                    .Where(x => x.EffectiveDate != null)
                    .GroupBy(x => new
                    {
                        Year = x.EffectiveDate!.Year,
                        Month = x.EffectiveDate!.Month,
                        x.Booking.FacilityId,
                        FacilityName = x.Booking.Facility.FacilityName
                    })
                    .Select(g => new FacilityUsageReportDto
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1)
                                    .ToString("MMM-yy"),

                        Facility = g.Key.FacilityName,

                        TotalBookings = g.Count(),

                        UniqueResidents = g
                            .Select(x => x.Booking.ResidentId)
                            .Distinct()
                            .Count(),

                        MostActiveDay = g
                            .Where(x => x.EffectiveDate!=null)
                            .GroupBy(x => x.EffectiveDate!.DayOfWeek)
                            .OrderByDescending(d => d.Count())
                            .Select(d => d.Key.ToString())
                            .FirstOrDefault(),

                        RevenueCollected = g
                            .Where(x => x.PaymentStatus == "Paid")
                            .Sum(x => decimal.TryParse(x.Booking.Amount, out var v) ? v : 0),

                        PendingPayment = g
                            .Where(x => x.PaymentStatus == "Pending")
                            .Sum(x => decimal.TryParse(x.Booking.Amount, out var v) ? v : 0)
                    })
                    .OrderBy(x => DateTime.ParseExact(x.Month, "MMM-yy", null))
                    .ToList();

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<OutstandingMaintenanceReportDto>> GetMaintenanceReportAsync()
        {
            var query = from p in _context.MaintanencePaymentStatus
                        join r in _context.Resident
                            on p.ResidentId equals r.Id
                        where  p.PaymentStatus == "Pending"
                        select new
                        {
                            UnitNo = r.BlockNo+"-"+r.Level + "-" + r.HouseNo,
                            ResidentName = r.Name,
                            Amount = p.Amount
                        };

            var data = await query.ToListAsync(); // DB call ends here

            return data
                .Select(x => new OutstandingMaintenanceReportDto
                {
                    UnitNo = x.UnitNo,
                    ResidentName = x.ResidentName,
                    Amount = decimal.TryParse(x.Amount, out var amt) ? amt : 0
                })
                .ToList(); // ✅ NOT ToListAsync
        }



        public async Task<OutstandingReportSummaryDto> GetMaintenanceReportSummaryAsync()
        {
            var items = await GetMaintenanceReportAsync();

            return new OutstandingReportSummaryDto
            {
                Items = items,
                TotalAmount = items.Sum(x => x.Amount)
            };
        }

        public async Task<List<MonthlyVisitorReportDto>> GetMonthlyVisitorReportAsync(
    int? communityId,
    int year)
        {
            var data = await _context.VisitorAccessDetails
     .Where(v=> v.VisitDate.HasValue            // ✅ null check
                 && v.VisitDate.Value.Year == year)
     .ToListAsync(); // materialize once

            var result = data
                .GroupBy(v => new
                {
                    Year = v.VisitDate!.Value.Year,
                    Month = v.VisitDate!.Value.Month
                })
                .Select(g =>
                {
                    var dailyCounts = g
                        .GroupBy(x => x.VisitDate!.Value.Date)
                        .Select(d => new
                        {
                            Date = d.Key,
                            Count = d.Count()
                        })
                        .ToList();

                    var peakDay = dailyCounts
                        .OrderByDescending(x => x.Count)
                        .First().Date;

                    return new MonthlyVisitorReportDto
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1)
                                    .ToString("MMM-yy"),

                        TotalVisitors = g.Count(),

                        UniqueVisitors = g
                            .Where(x => !string.IsNullOrEmpty(x.VisitorName))
                            .Select(x => x.VisitorName)
                            .Distinct()
                            .Count(),

                        AvgVisitorsPerDay = dailyCounts.Count == 0
                            ? 0
                            : Math.Round(
                                (decimal)g.Count() / dailyCounts.Count, 0),

                        PeakDay = peakDay.DayOfWeek.ToString()
                    };
                })
                .OrderBy(x => x.Month)
                .ToList();

            return result;

        }

        public async Task<List<PaymentSummaryDto>> GetCarParkCollectionReportAsync()
        {
            var fromDate = DateTime.UtcNow.AddDays(-30);

            var query =
                from pr in _context.PaymentRequest
                where pr.CreatedDate >= fromDate


                // LEFT join maintenance first
                join mps in _context.MaintanencePaymentStatus
                    on pr.ID equals mps.PaymentId into mpsJoin
                from mps in mpsJoin.DefaultIfEmpty()

                join vps in _context.VisitorPaymentStatus
                    on pr.ID equals vps.PaymentId into vpsJoin
                from vps in vpsJoin.DefaultIfEmpty()

                join fps in _context.FacilityPaymentStatus
                    on pr.ID equals fps.PaymentId into fpsJoin
                from fps in fpsJoin.DefaultIfEmpty()

                    // 🔑 CRITICAL FILTER
                where
                    mps != null               // valid maintenance payment
                    || vps != null            // OR valid visitor payment
                    || fps != null            // OR valid facility payment

                select new PaymentSummaryDto
                {
                    PaymentRequestId = pr.ID,
                    Amount = pr.Amount,
                    CreatedDate = pr.CreatedDate,
                    Invoice = pr.Invoice,

                    PaymentType =
                        mps != null ? "Maintenance" :
                        vps != null ? "Visitor" :
                        fps != null ? "Facility" :
                        "Unknown",

                    PaymentStatus =
                        mps != null ? mps.PaymentStatus :
                        vps != null ? vps.PaymentStatus :
                        fps != null ? fps.PaymentStatus :
                        "Pending"
                };

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }



    }
}
