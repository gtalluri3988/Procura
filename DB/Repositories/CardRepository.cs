using AutoMapper;
using DB.EFModel;
using DB.Entity;
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
    public class CardRepository : RepositoryBase<Card, CardDTO>, ICardRepository
    {
        public CardRepository(CSADbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }


        public async Task<IEnumerable<CardDTO>> GetAllResidentAccessCardsAsync()
        {
            var Cards = await _context.Card.Include(c => c.Resident).ToListAsync();
            return _mapper.Map<IEnumerable<CardDTO>>(Cards);
        }

        public async Task<CardDTO> GetResidentCardByIdAsync(int cardId)
        {
            var Cards = await _context.Card.Where(x=>x.Id== cardId).Include(c => c.Resident).FirstOrDefaultAsync();
            return _mapper.Map<CardDTO>(Cards);
        }
        public async Task UpdateCardDetailsAsync(int cardId, CardDTO dto)
        {
            var entity = await _context.Card
                               // If related data needs updating
                               .FirstOrDefaultAsync(c => c.Id == cardId);
            if (entity != null)
            {
                entity.CardNo = dto.CardNo;
                entity.UpdatedDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }
    }
}
