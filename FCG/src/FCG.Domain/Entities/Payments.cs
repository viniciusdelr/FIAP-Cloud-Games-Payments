using Microsoft.EntityFrameworkCore;

namespace FCG.Domain.Entities
{
    public class Payments
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string GameTitle { get; set; } = string.Empty;
        public int IdGame { get; set; }

        [Precision(18, 2)]
        public required decimal Price { get; set; }
        [Precision(18, 2)]
        public decimal DiscountPrice { get; set; }
        public int DiscountPerc { get; set; }

    }
}