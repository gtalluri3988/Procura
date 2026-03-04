using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class FacilityDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? FacilityName { get; set; }
        public string? FacilityLocation { get; set; }
        public int CommunityId { get; set; }
        public string? LotAvilability { get; set; }
        public string? FacilityDetails { get; set; }
        public int FacilityTypeId { get; set; }
        public string? Rate { get; set; }
        public bool? disableRateDeposit { get; set; }
        public string? Deposit { get; set; }
       
        public CommunityDTO? community { get; set; }
        
        



    }
}
