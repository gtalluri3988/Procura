using DB.EFModel;

namespace DB.Repositories
{
    using AutoMapper;
    using DB.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class RepositoryBase<TEntity, TDto> where TEntity : class
    {
        protected readonly ProcuraDbContext _context;
        protected readonly IMapper _mapper;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepositoryBase(ProcuraDbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _dbSet = _context.Set<TEntity>();
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("userid")?.Value ?? "Unknown";
        }
        public async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _dbSet.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
            return _mapper.Map<TDto>(entity);
        }

        public async Task UpdateAsync(int id, TDto dto)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserCommunity()
        {
            int userId = Convert.ToInt32(GetCurrentUserId());
            int? communityId = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            return communityId ?? 0;
        }



    }
}
