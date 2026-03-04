using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public Task<IEnumerable<CardDTO>> GetAllResidentCardsAsync()
        {
            return _cardRepository.GetAllResidentAccessCardsAsync();
        }

        public Task<CardDTO> GetCardByIdAsync(int id)
        {
            return _cardRepository.GetResidentCardByIdAsync(id);
        }

        public Task<CardDTO> SaveCardAsync(CardDTO dto)
        {
            return _cardRepository.AddAsync(dto);
        }

        public async Task UpdateCardAsync(int id, CardDTO dto)
        {
           await _cardRepository.UpdateCardDetailsAsync(id, dto);
        }
    }
}
