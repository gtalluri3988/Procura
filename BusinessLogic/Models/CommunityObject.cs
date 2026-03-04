using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class CommunityObject
    {
        public long Id { get; set; }  // Primary Key
        public int? State { get; set; }  // Primary Key
        public string? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public int NoOfUnits { get; set; }
        public string? PICName { get; set; }
        public string? PICPhone { get; set; }
        public string? PICEmail { get; set; }
        public int NoOfResidentParkingLot { get; set; }
        public List<TableData> TableRows { get; set; }

    }

    public class CommunityTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

   


    public class TableData
    {
        public long Id { get; set; } 
        public int NoOfVistorParkingLot { get; set; } 
        public int Amount { get; set; } 
        public int Status { get; set; } 
        public int ChargeType { get; set; } 
    }

    public class CommunityModel
    {
        public int CommunityType { get; set; } 
        public string? CommunityName { get; set; } = string.Empty;
        public TableData[]? TableRows { get; set; }
    }


}
