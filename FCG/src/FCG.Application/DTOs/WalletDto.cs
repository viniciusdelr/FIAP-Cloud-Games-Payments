using Microsoft.EntityFrameworkCore;

namespace FCG.Application.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }

        [Precision(18, 2)]
        public required decimal Funds { get; set; }
        public DateTime LastRecharge { get; set; }
    }
}
