namespace FCG.API.Shared.Events
{
    public record PaymentResult
    {
        public Guid CorrelationId { get; init; }
        public bool Success { get; init; }
        public string Message { get; init; }
    }
}
