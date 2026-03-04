using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IFacilityBookingService
    {
        Task<FacilityBookingDTO> CreateFacilityBookingAsync(FacilityBookingDTO dto);
        Task<FacilityBookingDTO?> GetResidentFacilityBookingById(int bookingId);

        Task<FacilityBookingDTO?> GetResidentFacilityBookingByFavilityId(int facilityId, int residentId);
        Task<List<FacilityBookingDTO>?> GetResidentFacilityBookingByFavilityId(int facilityId);
        Task<FacilityBookingDTO?> GetResidentFacilityBookingByBookingId(int bookingId);
        Task UpdateFacilityBookingAsync(int facilityBookingId, FacilityBookingDTO dto);
    }
}
