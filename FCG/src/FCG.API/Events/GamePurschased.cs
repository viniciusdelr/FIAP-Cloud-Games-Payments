namespace FCG.API.Events
{
    public record GamePurchased
    {
        public Guid CorrelationId { get; init; }
        public int UserId { get; init; }
        public decimal Price { get; init; }
    }
}
