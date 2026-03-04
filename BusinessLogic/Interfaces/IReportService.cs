using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IReportService
    {
        Task<List<MonthlyCollectionReportDto>> GetCollectionSummaryReportAsync(int? communityId);
        Task<List<FacilityUsageReportDto>> GetFacilityUsageReportAsync();
        Task<OutstandingReportSummaryDto> GetMaintenanceReportSummaryAsync();

        Task<List<MonthlyVisitorReportDto>> GetMonthlyVisitorReportAsync(
    int? communityId,
    int year);
        Task<List<PaymentSummaryDto>> GetCarParkCollectionReportAsync();
    }
}
