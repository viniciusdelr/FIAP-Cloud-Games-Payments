using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FCG.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public string Username { get; set; }
        public decimal Funds { get; set; }
        public DateTime LastRecharge { get; set; }

    }
}
