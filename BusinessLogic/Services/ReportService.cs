using BusinessLogic.Interfaces;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ReportService:IReportService
    {
        private readonly IReportRepository _repository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository repository, ILogger<ReportService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<MonthlyCollectionReportDto>> GetCollectionSummaryReportAsync(int? communityId)
        {
            return await _repository.GetCollectionSummaryReportAsync(communityId);
        }

        public async Task<List<FacilityUsageReportDto>> GetFacilityUsageReportAsync()
        {
            return await _repository.GetFacilityUsageReportAsync();
        }

        public async Task<OutstandingReportSummaryDto> GetMaintenanceReportSummaryAsync()
        {
            return await _repository.GetMaintenanceReportSummaryAsync();
        }

        public async Task<List<MonthlyVisitorReportDto>> GetMonthlyVisitorReportAsync(int? communityId, int year)
        {
            return await _repository.GetMonthlyVisitorReportAsync(communityId,year);
        }

        public async Task<List<PaymentSummaryDto>> GetCarParkCollectionReportAsync()
        {
            return await _repository.GetCarParkCollectionReportAsync();
        }


        //public async Task<string> GetCollectionSummaryReportAsync()
        //{
        //    var comId = await _context.Community.Where(x => x.CommunityId == communityId).FirstOrDefaultAsync();
        //    if (comId == null)
        //        throw new Exception("No community found");
        //    return comId?.AllowAccess;
        //}
    }
}
