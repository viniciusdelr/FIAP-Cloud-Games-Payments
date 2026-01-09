using FCG.Infrastructure.Data;
using FCG.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using FCG.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FCG.Controllers
{
    public class WalletController : Controller
    {
        private readonly DataContext _context;
        public record RechargeDto(int UserId, decimal Amount);
        public WalletController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Recharge")]
        [Authorize]//transformar em admin no futuro
        public async Task<IActionResult> Recharge([FromBody] RechargeDto dto)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == dto.UserId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = dto.UserId,
                    Username = "Usuario " + dto.UserId,
                    Funds = 0,
                    LastRecharge = DateTime.UtcNow
                };
                _context.Wallets.Add(wallet);
            }

            wallet.Funds += dto.Amount;
            wallet.LastRecharge = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Recarga realizada com sucesso!",
                NewBalance = wallet.Funds
            });
        }

        [HttpGet("GetWallet/{userId}")]
        [Authorize]

        public async Task<IActionResult> GetBalance(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null) return NotFound("Carteira não encontrada");

            return Ok(wallet);
        }

    }
}
