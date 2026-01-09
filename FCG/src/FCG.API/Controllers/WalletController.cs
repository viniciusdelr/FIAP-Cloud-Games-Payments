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
        public WalletController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("RechargeWallet")]
        public async Task<ActionResult> RechargeWallet([FromBody] WalletDto dtoWallet)
        {

            //var Payments = new Payments
            //{
            //    FirstName = dtoPayments.FirstName,
            //    GameTitle = dtoPayments.GameTitle,
            //    IdGame = dtoPayments.IdGame,
            //    Price = dtoPayments.Price,
            //    DiscountPerc = dtoPayments.DiscountPerc,
            //    DiscountPrice = dtoPayments.Price - (dtoPayments.Price * dtoPayments.DiscountPerc / 100)
            //};

            //_context.Payments.Add(Payments);
            //await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Compra Realizada!" });
        }

    }
}
