using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IFacilityBookingRepository
    {
        Task<IEnumerable<FacilityBookingDTO>> GetAllAsync();
        Task<FacilityBookingDTO> GetByIdAsync(int id);
        Task<FacilityBookingDTO> AddAsync(FacilityBookingDTO dto);
        Task UpdateAsync(int id, FacilityBookingDTO dto);
        Task DeleteAsync(int id);
        Task<FacilityBookingDTO> SaveFacilityBookinAsync(FacilityBookingDTO dto);
        Task<FacilityBookingDTO> GetResidentFacilityBookingByFavilityId(int facilityId, int residentId);
        Task<List<FacilityBookingDTO>> GetResidentFacilityBookingByFavilityId(int facilityId);

        Task<FacilityBookingDTO> GetResidentFacilityBookingByBookingId(int bookingId);
        Task UpdateFacilityBookingAsync(int facilityBookingId, FacilityBookingDTO facilityBooking);


    }
}
