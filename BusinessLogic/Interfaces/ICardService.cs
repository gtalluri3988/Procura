using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ICardService
    {

        Task UpdateCardAsync(int id, CardDTO dto);

        Task<CardDTO> SaveCardAsync(CardDTO dto);

        Task<CardDTO> GetCardByIdAsync(int id);

        Task<IEnumerable<CardDTO>> GetAllResidentCardsAsync();

    }
}
