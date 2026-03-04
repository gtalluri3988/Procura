using AutoMapper;
using Azure.Core;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Net.NetworkInformation;

namespace DB.Repositories
{
    public class FacilityRepository : RepositoryBase<Facility, FacilityDTO>, IFacilityRepository
    {
        
        public FacilityRepository(CSADbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }


        public async Task<IEnumerable<FacilityDTO>> GetAllFacilitiesAsync()
        {
            int communityId = await GetUserCommunity();
            var query = _context.Facility
                .Include(c => c.FacilityType)
                .Include(c => c.Community)
                .AsQueryable();
            if (communityId != 0)
            {
                query = query.Where(f => f.CommunityId == communityId);
            }
            var facilities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<FacilityDTO>>(facilities);
        }

        public async Task<IEnumerable<FacilityDTO>> SearchFacilitiesAsync(int communityId,int facilityTypeId)
        {
            var Facilities = await _context.Facility.Include(c => c.FacilityType).Include(x => x.Community).Where(x =>
        (facilityTypeId == 0 || x.FacilityTypeId == facilityTypeId) &&
        (communityId == 0 || x.CommunityId == communityId)).ToListAsync();
            return _mapper.Map<IEnumerable<FacilityDTO>>(Facilities);
        }

        public async Task<FacilityDTO> CreateFacilityAsync(FacilityDTO dto)
        {
            var entity = _mapper.Map<EFModel.Facility>(dto);
            _context.Facility.Add(entity);
            entity.FacilityPhotos.Clear();
            
            
            if (entity != null && dto.FacilityPhotos!=null)
            {
                foreach (var item in dto.FacilityPhotos)
                {
                    FacilityPhoto photo = new FacilityPhoto();
                    var base64Data =item.Preview== null?"": item.Preview.Split(',').Last();
                    var imageBytes = Convert.FromBase64String(base64Data);
                    var fileName = $"{Guid.NewGuid()}.png";
                    string drivePath = @"C:\Uploads\";

                    // Ensure the directory exists
                    if (!Directory.Exists(drivePath))
                    {
                        Directory.CreateDirectory(drivePath);
                    }
                    var filePath = Path.Combine(drivePath, fileName);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    var fileUrl = $"/uploads/{fileName}";
                    photo.ImageGuid = fileName;
                    photo.FacilityId=entity.Id;
                    photo.Name = "";
                    photo.Preview = "";
                    //_context.FacilityPhoto.Add(photo);
                    entity.FacilityPhotos.Add(photo);
                }
               
            }
            await _context.SaveChangesAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<FacilityDTO> GetAllFacilityByIdAsync(int id)
        {
            var Facility = await _context.Facility.Where(x=>x.Id==id).Include(c => c.FacilityType).Include(x => x.Community).Include(x=>x.FacilityPhotos)
               .FirstOrDefaultAsync();
            if (Facility != null)
            {
               
               foreach (var res in Facility.FacilityPhotos) {
                    var image = res.ImageGuid ==null ? "" : res.ImageGuid;
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                                     .Where(f => Path.GetFileName(f)
                                     .Contains(image, StringComparison.OrdinalIgnoreCase))
                                     .ToList();
                    if (matchingFiles.Count > 0)
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(matchingFiles[0].ToString());
                        string base64String = Convert.ToBase64String(imageBytes);
                        res.Preview = "data:image/" + Path.GetExtension(matchingFiles[0].ToString()) + ";base64," + base64String;
                        res.Name = Path.GetFileName(matchingFiles[0].ToString());
                    }
                }
            }
            return _mapper.Map<FacilityDTO>(Facility);
        }

        public async Task<FacilityDTO> GetAllFacilityByAvilableLotQtyIdAsync(int id)
        {
            var Facility = await _context.Facility.Where(x => x.Id == id).Include(c => c.FacilityType).Include(x => x.Community).Include(x => x.FacilityPhotos)
               .FirstOrDefaultAsync();
            if (Facility != null)
            {
               
                Facility.LotAvilability = (Convert.ToInt32(Facility.LotAvilability)- Convert.ToInt32(LotAvilability(id))) + " / " + Facility.LotAvilability;

                foreach (var res in Facility.FacilityPhotos)
                {
                    var image = res.ImageGuid == null ? "" : res.ImageGuid;
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                                     .Where(f => Path.GetFileName(f)
                                     .Contains(image, StringComparison.OrdinalIgnoreCase))
                                     .ToList();
                    if (matchingFiles.Count > 0)
                    {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(matchingFiles[0].ToString());
                        string base64String = Convert.ToBase64String(imageBytes);
                        res.Preview = "data:image/" + Path.GetExtension(matchingFiles[0].ToString()) + ";base64," + base64String;
                        res.Name = Path.GetFileName(matchingFiles[0].ToString());
                    }
                }
            }
            return _mapper.Map<FacilityDTO>(Facility);
        }

        public async Task<string> GetLotAvilabilityByMonth(string startMonth, int facilityId)
        {
            var Facility = await _context.Facility.Where(x => x.Id == facilityId).Include(c => c.FacilityType).Include(x => x.Community).Include(x => x.FacilityPhotos)
              .FirstOrDefaultAsync();
            if (Facility != null)
            {
                Facility.LotAvilability = (Convert.ToInt32(Facility.LotAvilability) - Convert.ToInt32(LotAvilabilityByStartMonth(startMonth,facilityId))) + " / " + Facility.LotAvilability;
                return Facility.LotAvilability;
            }
            return "";
        }
        private string LotAvilabilityByStartMonth(string startMonth, int? facilityId)
        {
            if (facilityId == null || _context.ResidentFacilityBooking == null)
                return "0";
            var lotSum = _context.ResidentFacilityBooking
                .Where(x => x.FacilityId == facilityId && x.RentalStartMonth == startMonth)
                .Select(x => (int?)x.LotQuantity)
                .Sum() ?? 0;
            return lotSum.ToString();
        }


        private string LotAvilability(int? facilityId)
        {
            if (facilityId == null || _context.ResidentFacilityBooking == null)
                return "0";

            var lotSum = _context.ResidentFacilityBooking
                .Where(x => x.FacilityId == facilityId && x.RentalStartMonth==DateTime.Now.Month.ToString())
                .Select(x => (int?)x.LotQuantity)
                .Sum() ?? 0;

            return lotSum.ToString();
        }


        public async Task UpdateFacilityAsync(int facilityId, FacilityDTO facility)
        {

            var entity = await _context.Facility
                                // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == facilityId);
            if (entity != null)
            {
                entity.FacilityDetails = facility.FacilityDetails;
                entity.UpdatedDate = DateTime.Now;
                entity.Rate = facility.Rate;
                entity.FacilityLocation = facility.FacilityLocation;
                entity.FacilityName = facility.FacilityName;
                entity.FacilityTypeId = facility.FacilityTypeId;
                entity.Deposit = facility.Deposit;
                entity.LotAvilability= facility.LotAvilability;

               
                if (facility.FacilityPhotos != null)
                {
                    _context.FacilityPhoto.RemoveRange(_context.FacilityPhoto.Where(p => p.FacilityId == facilityId));
                    await _context.SaveChangesAsync();
                    foreach (var item in facility.FacilityPhotos)
                    {
                        FacilityPhoto photo = new FacilityPhoto();
                        var base64Data = item.Preview == null ? "" : item.Preview.Split(',').Last();
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var fileName = $"{Guid.NewGuid()}.png";
                        string drivePath = @"C:\Uploads\";

                        // Ensure the directory exists
                        if (!Directory.Exists(drivePath))
                        {
                            Directory.CreateDirectory(drivePath);
                        }
                        var filePath = Path.Combine(drivePath, fileName);
                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        var fileUrl = $"/uploads/{fileName}";
                        photo.ImageGuid = fileName;
                        photo.FacilityId = entity.Id;
                        photo.Name = "";
                        photo.Preview = "";
                        entity.FacilityPhotos.Add(photo);

                    }
                }
            }
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByCommunityAsync(int communityId)
        {
           List<FecilityMobile> lstMobile=new List<FecilityMobile>();
            var facilities = await _context.Facility
                .Where(x => x.CommunityId == communityId)
                .Include(c => c.FacilityType)
                .Include(c => c.Community)
                .Include(c=>c.ResidentFacilityBookings)
                .Include(c => c.FacilityPhotos)
                .ToListAsync();

            foreach (var facility in facilities)
            {
                if (CheckAvilability(facility.Id))
                {
                    try
                    {

                        FecilityMobile mobile = new FecilityMobile();
                        FacilityPhoto facilityPhoto = new FacilityPhoto();
                        mobile.FacilityName = facility.FacilityName;
                        mobile.FacilityTypeId = facility.FacilityTypeId;
                        mobile.Id = facility.Id;
                        var photo = facility.FacilityPhotos.FirstOrDefault();
                        if (photo != null && !string.IsNullOrEmpty(photo.ImageGuid))
                        {
                            var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                                .Where(f => Path.GetFileName(f)
                                .Contains(photo.ImageGuid, StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            if (matchingFiles.Any())
                            {
                                var filePath = matchingFiles[0];
                                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                                string base64String = Convert.ToBase64String(imageBytes);
                                string ext = Path.GetExtension(filePath).TrimStart('.');

                                photo.Preview = $"data:image/{ext};base64,{base64String}";
                                photo.Name = Path.GetFileName(filePath);
                                facilityPhoto.Preview = $"data:image/{ext};base64,{base64String}";
                                facilityPhoto.Name = Path.GetFileName(filePath);
                                mobile.FacilityPhotos = facilityPhoto;
                            }
                        }
                        lstMobile.Add(mobile);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return lstMobile;
        }



        public async Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByCommunityAsync(int communityId)
        {
            List<FecilityMobile> lstMobile = new List<FecilityMobile>();
            var facilities = await _context.Facility
                .Where(x => x.CommunityId == communityId)
                .Include(c => c.FacilityType)
                .Include(c => c.Community)
                .Include(c => c.ResidentFacilityBookings)
                .Include(c => c.FacilityPhotos)
                .ToListAsync();
            return _mapper.Map<List<FacilityDTO>>(facilities);
        }


        public async Task<IEnumerable<FacilityDTO>> GetAllFacilityHistoryByResidentAsync(int residentId)
        {
            var currentUserId = Convert.ToInt32(GetCurrentUserId());

            var facilities = await _context.Facility
                .Include(x => x.ResidentFacilityBookings)
                .Include(c => c.FacilityType)
                .Include(c => c.Community)
                .Include(c => c.FacilityPhotos)
                .Where(x => x.ResidentFacilityBookings.Any(r => r.ResidentId == currentUserId))
                .OrderByDescending(x => x.ResidentFacilityBookings
                    .Max(r => r.StartDate))   // ✅ order by latest booking date
                .ToListAsync();

            return _mapper.Map<List<FacilityDTO>>(facilities);
        }






        //public async Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByCommunityAsync(int communityId)
        //{
        //    var facilities = await _context.Facility
        //        .Where(x => x.CommunityId == communityId)
        //        .Include(c => c.FacilityType)
        //        .Include(c => c.Community)
        //        .Include(c => c.FacilityPhotos)
        //        .ToListAsync();

        //    var result = new List<FecilityMobile>();

        //    foreach (var facility in facilities)
        //    {
        //        if (!CheckAvilability(facility.Id)) continue;

        //        var mobile = new FecilityMobile
        //        {
        //            FacilityName = facility.FacilityName,
        //            FacilityTypeId = facility.FacilityTypeId,
        //            Id = facility.Id
        //        };

        //        var photo = facility.FacilityPhotos.FirstOrDefault();
        //        if (photo != null && !string.IsNullOrEmpty(photo.ImageGuid))
        //        {
        //            mobile.FacilityPhotos = new FacilityPhoto
        //            {
        //                Preview = $"{_configuration["BaseUrl"]}/api/facility/image/{photo.ImageGuid}", // <-- Use real domain
        //                Name = photo.ImageGuid
        //            };
        //        }

        //        result.Add(mobile);
        //    }

        //    return result;
        //}

        public bool CheckAvilability(int facilityId)
        {
            // Get LotAvailability from Facility
            var lotAvailabilityString = _context.Facility
                                        .Where(f => f.Id == facilityId)
                                        .Select(f => f.LotAvilability)
                                        .FirstOrDefault();

            int lotAvailability = 0;

            if (!string.IsNullOrEmpty(lotAvailabilityString) && int.TryParse(lotAvailabilityString, out int parsedValue))
            {
                lotAvailability = parsedValue;
            }

            // Get sum of LotQuantity from ResidentFacilityBooking
            var bookedLots = _context.ResidentFacilityBooking
                    .Where(r => r.FacilityId == facilityId && r.RentalStartMonth == DateTime.Now.Month.ToString())
                    .Sum(r => (int?)r.LotQuantity) ?? 0; // use nullable to avoid null error
                var remainingLots = Convert.ToInt32(lotAvailability) - bookedLots;
               if (remainingLots > 0) {
                return true;
            }
               else
            {
                return false;
            }
        }


        public async Task<IEnumerable<FecilityMobile>> GetAllFacilitiesByfacilityTypeAsync(int communityId,int facilityTypeId)
        {
            List<FecilityMobile> lstMobile = new List<FecilityMobile>();
            var facilities = await _context.Facility
                .Where(x => x.CommunityId == communityId && x.FacilityTypeId== facilityTypeId)
                .Include(c => c.FacilityType)
                .Include(c => c.Community)

                .Include(c => c.FacilityPhotos)
                .ToListAsync();

            foreach (var facility in facilities)
            {
                FecilityMobile mobile = new FecilityMobile();
                FacilityPhoto facilityPhoto = new FacilityPhoto();
                mobile.FacilityName = facility.FacilityName;
                mobile.FacilityTypeId = facility.FacilityTypeId;
                mobile.Id = facility.Id;
                var photo = facility.FacilityPhotos.FirstOrDefault();
                if (photo != null && !string.IsNullOrEmpty(photo.ImageGuid))
                {
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                        .Where(f => Path.GetFileName(f)
                        .Contains(photo.ImageGuid, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matchingFiles.Any())
                    {
                        var filePath = matchingFiles[0];
                        byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                        string base64String = Convert.ToBase64String(imageBytes);
                        string ext = Path.GetExtension(filePath).TrimStart('.');

                        photo.Preview = $"data:image/{ext};base64,{base64String}";
                        photo.Name = Path.GetFileName(filePath);
                        facilityPhoto.Preview = $"data:image/{ext};base64,{base64String}";
                        facilityPhoto.Name = Path.GetFileName(filePath);
                        mobile.FacilityPhotos = facilityPhoto;
                    }
                }
                lstMobile.Add(mobile);
            }

            return lstMobile;
        }

        public async Task<IEnumerable<FecilityMobile>> GetFacilitiyImageByfacilityIdAsync(int communityId, int facilityTypeId)
        {
            List<FecilityMobile> lstMobile = new List<FecilityMobile>();
            var facilities = await _context.Facility
                .Where(x => x.CommunityId == communityId && x.FacilityTypeId == facilityTypeId)
                .Include(c => c.FacilityType)
                .Include(c => c.Community)

                .Include(c => c.FacilityPhotos)
                .ToListAsync();

            foreach (var facility in facilities)
            {
                FecilityMobile mobile = new FecilityMobile();
                FacilityPhoto facilityPhoto = new FacilityPhoto();
                mobile.FacilityName = facility.FacilityName;
                mobile.FacilityTypeId = facility.FacilityTypeId;
                mobile.Id = facility.Id;
                var photo = facility.FacilityPhotos.FirstOrDefault();
                if (photo != null && !string.IsNullOrEmpty(photo.ImageGuid))
                {
                    var matchingFiles = Directory.GetFiles(@"C:\Uploads\")
                        .Where(f => Path.GetFileName(f)
                        .Contains(photo.ImageGuid, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matchingFiles.Any())
                    {
                        var filePath = matchingFiles[0];
                        byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                        string base64String = Convert.ToBase64String(imageBytes);
                        string ext = Path.GetExtension(filePath).TrimStart('.');

                        photo.Preview = $"data:image/{ext};base64,{base64String}";
                        photo.Name = Path.GetFileName(filePath);
                        facilityPhoto.Preview = $"data:image/{ext};base64,{base64String}";
                        facilityPhoto.Name = Path.GetFileName(filePath);
                        mobile.FacilityPhotos = facilityPhoto;
                    }
                }
                lstMobile.Add(mobile);
            }

            return lstMobile;
        }

        public async Task<bool> DeleteFacility(int facilityId)
        {
            var facility = await _context.Facility
                        .FirstOrDefaultAsync(p => p.Id == facilityId);

            if (facility != null)
            {
                try
                {
                    var facilityDetails=_context.ResidentFacilityBooking.Where(p => p.FacilityId==facilityId).ToList();
                    foreach(var item in facilityDetails)
                    {
                        _context.ResidentFacilityBooking.Remove(item);
                    }
                    _context.FacilityPhoto.RemoveRange(facility.FacilityPhotos); // Delete child records
                    _context.Facility.Remove(facility); // Delete parent

                    _context.SaveChanges(); // Commit transaction
                }
                catch (Exception ex) { }
                return true;
            }
            return false;
        }

    }
}
