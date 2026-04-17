using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Helper;
using DB.Model;
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
    public class AnnouncementRepository : RepositoryBase<Announcement, AnnouncementDto>, IAnnouncementRepository
    {
        public AnnouncementRepository(ProcuraDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }

        public async Task<List<AnnouncementDto>> GetAnnouncementsAsync(AnnouncementType type)
        {
            return await _context.Announcements
                .Where(x => x.Type == type)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new AnnouncementDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Reference = x.Reference,
                    Type = x.Type,
                    Date = x.Date,
                    ClosingDate = x.ClosingDate,
                    VendorId = x.VendorId,
                    Value = x.Value,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy != null
                        ? _context.Users.Where(u => u.Id.ToString() == x.CreatedBy).Select(u => u.FullName).FirstOrDefault() ?? x.CreatedBy
                        : "",
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<AnnouncementDto> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements
                .Where(x => x.Id == id)
                .Select(x => new AnnouncementDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Reference = x.Reference,
                    Type = x.Type,
                    Date = x.Date,
                    ClosingDate = x.ClosingDate,
                    VendorId = x.VendorId,
                    Value = x.Value,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate
                })
                .FirstOrDefaultAsync() ?? new AnnouncementDto();
        }

        public async Task UpdateAnnouncementAsync(AnnouncementDto dto)
        {
            var entity = await _context.Announcements
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Announcement not found");

            entity.Title = dto.Title;
            entity.Reference = dto.Reference;
            entity.Type = dto.Type;
            entity.Date = dto.Date;
            entity.ClosingDate = dto.ClosingDate;
            entity.VendorId = dto.VendorId;
            entity.Value = dto.Value;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnnouncementAsync(int id)
        {
            var entity = await _context.Announcements
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity != null)
            {
                _context.Announcements.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AnnouncementDto>> SearchAnnouncementsAsync(string keyword)
        {
            return await _context.Announcements
                .Where(x => x.Title.Contains(keyword))
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new AnnouncementDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Reference = x.Reference,
                    Type = x.Type,
                    Date = x.Date,
                    Description = x.Description,
                    Status = x.Status,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<int> AddAnnouncementAsync(AnnouncementDto dto)
        {
            var entity = new Announcement
            {
                Title = dto.Title,
                Reference = dto.Reference,
                Type = dto.Type,
                Date = dto.Date ?? DateTime.UtcNow,
                ClosingDate = dto.ClosingDate,
                VendorId = dto.VendorId,
                Value = dto.Value,
                Description = dto.Description,
                Status = dto.Status ?? true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = GetCurrentUserId()
            };

            await _context.Announcements.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }
    }
}
