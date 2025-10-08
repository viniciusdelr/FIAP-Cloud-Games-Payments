using Microsoft.EntityFrameworkCore;

namespace FCG.Application.DTOs
{
    public class PaymentsDto
    {
        public required string FirstName { get; set; }
        public required string GameTitle { get; set; } = string.Empty;
        public int IdGame { get; set; }

        [Precision(18, 2)]
        public required decimal Price { get; set; }
        public int DiscountPerc { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
