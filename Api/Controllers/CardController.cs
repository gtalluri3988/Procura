using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DB.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Services;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CardController : AuthorizedCSABaseAPIController
    {
        private readonly ICardService _cardService;
        private readonly ICurrentUserService _currentUserService;

        public CardController(ICardService cardService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILogger<ResidentController> logger)
            : base(userService, logger)
        {
            _currentUserService = currentUserService;
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllResidentCards()
        {
            return Ok(await _cardService.GetAllResidentCardsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCardById(int id)
        {
            var Card = await _cardService.GetCardByIdAsync(id);
            if (Card == null)
                return NotFound();
            return Ok(Card);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CardDTO dto)
        {
            var card=await _cardService.SaveCardAsync(dto);
            return CreatedAtAction(nameof(GetCardById), new { id = card.Id }, card);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCard(int id, CardDTO dto)
        {
            await _cardService.UpdateCardAsync(id, dto);
            return NoContent();
        }

    }
}
