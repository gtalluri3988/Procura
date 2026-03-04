using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations.Helpers;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories
{
    public class FacilityBookingRepository : RepositoryBase<ResidentFacilityBooking, FacilityBookingDTO>, IFacilityBookingRepository
    {
        public FacilityBookingRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }

        public async Task<FacilityBookingDTO> SaveFacilityBookinAsync(FacilityBookingDTO dto)
        {
            var Facility = await _context.Facility.Where(x => x.Id == dto.FacilityId)
               .FirstOrDefaultAsync();
            if (Facility != null && Facility.FacilityTypeId==4)
            {
                if ((Convert.ToInt32(Facility.LotAvilability)-Convert.ToInt32(LotAvilability(dto.FacilityId,dto.RentalStartMonth))) -(dto.LotQuantity) < 0)
                {
                    throw new Exception("Lot Quantity should not be more than the current Lot Availability");
                }
            }
            if (Facility != null && Facility.FacilityTypeId != 4 && CheckOtherResidentBooking(dto.FacilityId,dto.StartDate,dto.EndDate))
            {
                throw new ArgumentException("Facility not avilable for these booking Dates ");
            }
            

            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("End date must be greater than start date.");

            var facility = await _context.Facility
                .FirstOrDefaultAsync(x => x.Id == dto.FacilityId);

            if (facility == null)
                throw new Exception("Facility not found.");

            int? days = (dto.StartDate.HasValue && dto.EndDate.HasValue)
    ? (int?)(dto.EndDate.Value - dto.StartDate.Value).Days
    : null;
            if(days!=null)
            dto.Amount = ((days+1) * Convert.ToDouble(facility.Rate)).ToString();

            var booking = new ResidentFacilityBooking
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ResidentId = dto.ResidentId,
                FacilityId = dto.FacilityId,
                Amount = dto.Amount,
                Deposit=facility.Deposit,
                CreatedDate = DateTime.Now,
                LotQuantity= dto.LotQuantity,
                RentalStartMonth= dto.RentalStartMonth,
                RentalStartYear= DateTime.Now.Year.ToString(),
            };

            _context.ResidentFacilityBooking.Add(booking);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(booking.Id);
        }


        
        private string LotAvilability(int? facilityId,string rentalStartMonth)
        {
            if (facilityId == null || _context.ResidentFacilityBooking == null)
                return "0";

            var lotSum = _context.ResidentFacilityBooking
                .Where(x => x.FacilityId == facilityId && x.RentalStartMonth == rentalStartMonth)
                .Select(x => (int?)x.LotQuantity)
                .Sum() ?? 0;

            return lotSum.ToString();
        }
        private bool CheckOtherResidentBooking(int? facilityId, DateTime? startDate, DateTime? endDate)
        {
            var BookingDetails = _context.ResidentFacilityBooking.Where(x => x.FacilityId == facilityId
                                && x.StartDate >= startDate && x.EndDate <= endDate).FirstOrDefault();
            if(BookingDetails!=null)
                return true;
            return false;
        }

        public async Task<FacilityBookingDTO> GetResidentFacilityBookingByFavilityId(int facilityId, int residentId)
        {

            var facilityBooking=await _context.ResidentFacilityBooking.OrderByDescending(x=>x.CreatedDate).Where(x=>x.ResidentId==residentId && x.FacilityId==facilityId).Include(x=>x.Resident)
                
                .Include(x=>x.Facility).ThenInclude(f => f.FacilityType).FirstOrDefaultAsync();
            return _mapper.Map<FacilityBookingDTO>(facilityBooking);

        }

        public async Task<List<FacilityBookingDTO>> GetResidentFacilityBookingByFavilityId(int facilityId)
        {

            var facilityBooking = await _context.ResidentFacilityBooking.OrderByDescending(x => x.CreatedDate).Where(x=> x.FacilityId == facilityId).Include(x => x.Resident)

                .Include(x => x.Facility).ThenInclude(f => f.FacilityType).ToListAsync();
            return _mapper.Map<List<FacilityBookingDTO>>(facilityBooking);

        }


        public async Task<FacilityBookingDTO> GetResidentFacilityBookingByBookingId(int bookingId)
        {
            var facilityBooking = await _context.ResidentFacilityBooking.OrderByDescending(x => x.CreatedDate).Where(x => x.Id == bookingId).Include(x => x.Resident)
                .Include(x => x.Facility).ThenInclude(f => f.FacilityType).FirstOrDefaultAsync();
            return _mapper.Map<FacilityBookingDTO>(facilityBooking);
        }

        public async Task UpdateFacilityBookingAsync(int facilityBookingId, FacilityBookingDTO facilityBooking)
        {
            var entity = await _context.ResidentFacilityBooking
                               // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == facilityBookingId);
            if (entity != null)
            {
                entity.IsDepositRefund = facilityBooking.IsDepositRefund;
                entity.RefundDateTime = DateTime.Now;
                entity.RefundAmount = facilityBooking.RefundAmount;
                entity.UpdatedDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
    }
}

