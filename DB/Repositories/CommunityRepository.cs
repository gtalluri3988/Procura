using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DB.Repositories
{
    public class CommunityRepository : RepositoryBase<Community, CommunityDTO>, ICommunityRepository
    {
        public CommunityRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<List<Community>?> GetCommunityListAsync()
        {
            int communityId = await GetUserCommunity();
            var query = _context.Community.Where(x=>x.Status==true).AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(c => c.Id == communityId);
            }
            return await query.ToListAsync();
        }
        public async Task<List<CommunityType>> GetCommunityTypeAsync()
        {
            return await _context.CommunityType.ToListAsync();
        }

        public async Task<IEnumerable<CommunityDTO>> GetAllWithStatesAsync()
        {
            int communityId = await GetUserCommunity();

            var query = _context.Community.Where(x => x.Status == true)
                .Include(c => c.State)
                .Include(c => c.City)
                .Include(c => c.CommunityType)
                .AsQueryable();

            if (communityId != 0)
            {
                query = query.Where(c => c.Id == communityId);
            }

            var communityList = await query.OrderByDescending(x => x.Id).ToListAsync();

            return _mapper.Map<IEnumerable<CommunityDTO>>(communityList);
        }

        public async Task<CommunityDTO> SaveCommunityAsync(CommunityDTO community)
        {
            var entity = _mapper.Map<EFModel.Community>(community);
            _context.Community.Add(entity);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task UpdateCommunityAsync(int communityId, CommunityDTO community)
        {

            var entity = await _context.Community.Where(x => x.Status == true)
                               .Include(c => c.VisitorParkingCharges) // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == communityId);
            if (entity != null)
            {
                entity.Address = community.Address;
                entity.UpdatedDate = DateTime.Now;
                entity.CityName = community.CityName;
                entity.CityId = community.CityId;
                entity.PICName = community.PICName;
                entity.CommunityName = community.CommunityName;
                entity.CommunityTypeId = community.CommunityTypeId;
                entity.StateId = community.StateId;
                entity.FeesMonthly = community.FeesMonthly;
                entity.GracePeriod = community.GracePeriod;
                entity.NoOfParkingLot = 1;
                entity.NoOfUnits = community.NoOfUnits;
                entity.PICMobile = community.PICMobile;
                entity.PICEmail = community.PICEmail;
                entity.SinkingFund= community.SinkingFund;
                entity.SOS = community.SOS;
                entity.AllowAccess = community.AllowAccess;
                entity.maxDailyRate = community.maxDailyRate;
                entity.firstHour = community.firstHour;
                entity.subsequentHour = community.subsequentHour;
                entity.overnight = community.overnight;
                entity.shortVisit = community.shortVisit;


                if (community.VisitorParkingCharges != null)
                {
                    entity.VisitorParkingCharges.Clear();
                    foreach (var charge in community.VisitorParkingCharges)
                    {
                        entity.VisitorParkingCharges.Add(new VisitorParkingCharge
                        {
                            ChargeTypeId = charge.ChargeTypeId,
                            Amount = charge.Amount,
                            NoOfVistorParkingLot = charge.NoOfVistorParkingLot,
                            Status = charge.Status
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();

        }
        public async Task<CommunityDTO> GetCommunityByIdAsync(int id)
        {
            var complaints = await _context.Community.Where(x => x.Status == true).Include(c => c.State).Include(c=>c.City).Include(c => c.VisitorParkingCharges).Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CommunityDTO>(complaints);
        }


        public async Task<List<CommunityResidentCountDto>> GetAllCommunityWithResidentListAsync()
        {
            int communityId = await GetUserCommunity();

            var query = _context.Community.Where(x => x.Status == true).AsQueryable();

            if (communityId != 0)
            {
                query = query.Where(c => c.Id == communityId);
            }

            var result = await query
                .Select(c => new CommunityResidentCountDto
                {
                    Id = c.Id,
                    CommunityId = c.CommunityId,
                    CommunityName = c.CommunityName,
                    ResidentCount = c.Residents.Count()
                })
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return result;
        }


        public async Task<List<Community>> GetCommunitiesWithResidentsAsync()
        {
            return await _context.Community.Where(x => x.Status == true)
                .Include(c => c.Residents)  // Include Residents
                .ToListAsync();
        }

        public async Task<Community?> GetCommunityByIdWithResidentsAsync(int id)
        {
            return await _context.Community
                .Where(c => c.Id == id && c.Status == true)
                .Include(c => c.Residents)  // Load residents for this community
                .FirstOrDefaultAsync();
        }

        public async Task<string> IncrementAndGetNextNumberAsync()
        {
            var record = await _context.Community.OrderByDescending(x => x.Id).Select(x => x.CommunityId).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(record))
                return "001";
            else
                return (int.Parse(record) + 1).ToString("D3");
        }

        public async Task<IEnumerable<DropDownDTO>> GetCityByStateAsync(int stateId)
        {
            return await _context.City.OrderBy(x => x.Name).Where(x => x.StateId == stateId)
                .Select(c => new DropDownDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteCommunity(int communityId)
        {
            var community =await _context.Community
                        .Include(p => p.VisitorParkingCharges).Include(p=>p.Residents).Include(x=>x.Users) // Load related data
                        .FirstOrDefaultAsync(p => p.Id == communityId);

            if (community != null)
            {
                try
                {
                    community.Status = false; // Soft delete

                    _context.SaveChanges(); // Commit transaction
                }
                catch(Exception ex) { }
                return true;
            }
            return false;
        }

       

        public string GetCommunityNameByIdAsync(int communityId)
        {
            return _context.Community.Where(x => x.Status == true)
                           .AsNoTracking()
                           .Where(x => x.Id == communityId)
                           .Select(x => x.CommunityName)
                           .FirstOrDefault() ?? string.Empty;
        }
    }
}
