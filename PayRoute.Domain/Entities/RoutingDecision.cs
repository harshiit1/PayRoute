namespace PayRoute.Domain.Entities
{
    public class RoutingDecision
    {
        public Guid RoutingId { get; init; } = Guid.NewGuid();
        public string RecommendedMethodId { get; init;  }
        public List<RankedMethod> Ranking { get; init; } = [];
        public string Reason { get; init;  }
        public DateTime DecidedAt { get; init; } = DateTime.UtcNow;
    }

    public class RankedMethod
    {
        public string MethodId { get; init; }
        public double EstimatedSuccessRate { get; init;  }
        public int Rank { get; init; }
    }
}
