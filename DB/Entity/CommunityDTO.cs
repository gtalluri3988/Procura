using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class CommunityDTO
    {
        public int Id { get; set; }  // Primary Key
        public int? StateId { get; set; }  // Primary Key
        public string? CommunityId { get; set; }
        public int CityId { get; set; }
        public string? CommunityName { get; set; }
        public string? CityName { get; set; }
        public string? Address { get; set; }
        public int NoOfUnits { get; set; }
        public bool? Status { get; set; }
        public string? PICName { get; set; }
        public string? FeesMonthly { get; set; }
        public string? SinkingFund { get; set; }
        public int GracePeriod { get; set; }
        public string? PICMobile { get; set; }
        public string? PICEmail { get; set; }
        public string? SOS { get; set; }
        public string? AllowAccess { get; set; }
        public int CommunityTypeId { get; set; }
        public string? shortVisit { get; set; }
        public string? firstHour { get; set; }
        public string? subsequentHour { get; set; }
        public string? maxDailyRate { get; set; }
        public string? overnight { get; set; }
       
        public State? State { get; set; }
        
        public City? City { get; set; }
        public IEnumerable<CommunityDTO>? CommunityResult { get; set; }
    }

    public class TableData
    {
        public long Id { get; set; }
        public int NoOfVistorParkingLot { get; set; }
        public int Amount { get; set; }
        public int Status { get; set; }
        public int ChargeType { get; set; }
    }
}
