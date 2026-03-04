using AutoMapper;
using Azure.Core;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Net.NetworkInformation;

namespace DB.Repositories
{
    public class ContentRepository : RepositoryBase<ContentManagement, ContentManagementDTO>, IContentRepository
    {

        public ContentRepository(ProcuraDbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }


        public async Task<IEnumerable<ContentManagementDTO>> GetAllContentsAsync()
        {
            var residents = await _context.ContentManagement.OrderByDescending(x=>x.Id).ToListAsync();
            return _mapper.Map<IEnumerable<ContentManagementDTO>>(residents);
        }

        public async Task<ContentManagementDTO> CreateContentAsync(ContentManagementDTO dto)
        {
            var entity = _mapper.Map<EFModel.ContentManagement>(dto);
            _context.ContentManagement.Add(entity);
            entity.ContentPictures.Clear();


            if (entity != null && dto.ContentPictures != null)
            {
                foreach (var item in dto.ContentPictures)
                {
                    ContentPicture photo = new ContentPicture();
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
                    photo.ContentManagementId = entity.Id;
                    photo.Name = "";
                    photo.Preview = "";
                    //_context.FacilityPhoto.Add(photo);
                    entity.ContentPictures.Add(photo);
                }

            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return await GetByIdAsync(entity.Id);
        }

        public async Task<ContentManagementDTO> GetAllContentByIdAsync(int id)
        {
            var Content = await _context.ContentManagement.Where(x => x.Id == id).Include(x => x.ContentPictures).FirstOrDefaultAsync();
            if (Content != null)
            {
                foreach (var res in Content.ContentPictures)
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
            return _mapper.Map<ContentManagementDTO>(Content);
        }

        public async Task UpdateContentAsync(int contentManagementId, ContentManagementDTO content)
        {

            var entity = await _context.ContentManagement
                               // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == contentManagementId);
            if (entity != null)
            {
                entity.Title = content.Title;
                entity.UpdatedDate = DateTime.Now;
                entity.Description = content.Description;
                entity.StatusId = content.StatusId;
                


                if (content.ContentPictures != null)
                {
                    _context.ContentPictures.RemoveRange(_context.ContentPictures.Where(p => p.ContentManagementId == contentManagementId));
                    await _context.SaveChangesAsync();
                    foreach (var item in content.ContentPictures)
                    {
                        ContentPicture photo = new ContentPicture();
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
                        photo.ContentManagementId = entity.Id;
                        photo.Name = "";
                        photo.Preview = "";
                        entity.ContentPictures.Add(photo);

                    }
                }
            }
            await _context.SaveChangesAsync();

        }
    }
}
