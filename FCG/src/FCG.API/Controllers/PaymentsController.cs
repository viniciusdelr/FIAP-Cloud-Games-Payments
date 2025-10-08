using FCG.Infrastructure.Data;
using FCG.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using FCG.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FCG.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly DataContext _context;
        public PaymentsController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("PurchaseVideoGame")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostVideoGames([FromBody] PaymentsDto dtoPayments)
        {

            var Payments = new Payments
            {
                FirstName = dtoPayments.FirstName,
                GameTitle = dtoPayments.GameTitle,
                IdGame = dtoPayments.IdGame,
                Price = dtoPayments.Price,
                DiscountPerc = dtoPayments.DiscountPerc,
                DiscountPrice = dtoPayments.Price - (dtoPayments.Price * dtoPayments.DiscountPerc / 100)
            };

            _context.Payments.Add(Payments);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Compra Realizada!" });
        }

        
        [HttpGet("SearchUserPurchases")]
        [Authorize]
        public async Task<ActionResult> GetVideoGames()
        {
            var payments = await _context.Payments.ToListAsync();

            return Ok(payments);
        }

        [HttpGet("SearchPurchaseById/{id}")]
        [Authorize]
        public async Task<ActionResult> GetVideoGames(int id)
        {
            //alterar rotina
            var payments = await _context.Payments.FindAsync(id);

            if (payments is null)
                return NotFound(new { mensagem = "Jogo não encontrado." });

            return Ok(payments);
        }
    }
}
