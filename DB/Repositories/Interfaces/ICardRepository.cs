using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface ICardRepository
    {
        Task<IEnumerable<CardDTO>> GetAllAsync();
        Task<CardDTO> GetByIdAsync(int id);
        Task<CardDTO> AddAsync(CardDTO dto);
        Task UpdateAsync(int id, CardDTO dto);
        Task DeleteAsync(int id);

        Task<IEnumerable<CardDTO>> GetAllResidentAccessCardsAsync();
        Task<CardDTO> GetResidentCardByIdAsync(int cardId);
        Task UpdateCardDetailsAsync(int cardId, CardDTO dto);
    }
}
