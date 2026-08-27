namespace PayRoute.Domain.Entities
{
    public class PaymentAttempt
    {
        public Guid PaymentId { get; init; } = Guid.NewGuid();
        public string PaymentMethodId { get; init;  }
        public decimal OrderValue { get; init; }
        public string UserSegment { get; init; }             // "NEW", "RETURNING", "VIP"
        public int HourOfDay { get; init;  }                 // 0-23
        public bool Success { get; init; }
        public double LatencyMs { get; init; }
        public DateTime AttemptedAt { get; init; } = DateTime.UtcNow;
    }
}
