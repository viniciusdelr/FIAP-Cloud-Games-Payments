namespace FCG.API.Shared.Events
{
    public record GamePurchased
    {
        public Guid Id { get; init; }
        public int UserId { get; init; }
        public decimal Price { get; init; }
    }
}
