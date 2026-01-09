namespace FCG.API.Events
{
    public record PaymentResult
    {
        public Guid CorrelationId { get; init; }
        public bool Success { get; init; }
        public string Message { get; init; }
    }
}
