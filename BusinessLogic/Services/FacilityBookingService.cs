using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLogic.Services
{
    public class FacilityBookingService : IFacilityBookingService
    {
        private readonly IFacilityBookingRepository _fecilityBookingRepository;
        public FacilityBookingService(IFacilityBookingRepository facilityBookingRepository)
        {
            _fecilityBookingRepository = facilityBookingRepository;
        }
        public async Task<FacilityBookingDTO> CreateFacilityBookingAsync(FacilityBookingDTO dto)
        {
            return await _fecilityBookingRepository.SaveFacilityBookinAsync(dto);
        }
        public async Task<FacilityBookingDTO?> GetResidentFacilityBookingById(int bookingId)
        {
            return await _fecilityBookingRepository.GetByIdAsync(bookingId);
        }
        public async Task<FacilityBookingDTO?> GetResidentFacilityBookingByFavilityId(int facilityId,int residentId)
        {
            return await _fecilityBookingRepository.GetResidentFacilityBookingByFavilityId(facilityId,residentId);
        }

        public async Task<FacilityBookingDTO?> GetResidentFacilityBookingByBookingId(int bookingId)
        {
            return await _fecilityBookingRepository.GetResidentFacilityBookingByBookingId(bookingId);
        }

        public async Task<List<FacilityBookingDTO>?> GetResidentFacilityBookingByFavilityId(int facilityId)
        {
            return await _fecilityBookingRepository.GetResidentFacilityBookingByFavilityId(facilityId);
        }

        public async Task UpdateFacilityBookingAsync(int facilityBookingId,FacilityBookingDTO dto)
        {
             await _fecilityBookingRepository.UpdateFacilityBookingAsync(facilityBookingId,dto);
        }
    }
}
