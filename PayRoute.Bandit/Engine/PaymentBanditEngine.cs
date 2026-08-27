using PayRoute.Bandit.Models;
using PayRoute.Domain.Entities;

namespace PayRoute.Bandit.Engine
{
    public class PaymentBanditEngine
    {
        private readonly Dictionary<string, Dictionary<string, BanditArm>> _contextArms = new();
        private readonly List<string> _methodIds;
        private int _totalRounds = 0;
        private readonly object _lock = new();                              // for thread safety

        public PaymentBanditEngine(IEnumerable<string> methodIds)
        {
            _methodIds = methodIds.ToList();
        }

        public RoutingDecision Recommend(BanditContext context)
        {
            lock (_lock)
            {
                _totalRounds++;
                var arms = GetOrCreateArms(context.ContextKey);

                var ranked = arms.Values.OrderByDescending(a => a.UCBScore(_totalRounds))
                    .Select((a, i) => new RankedMethod
                    {
                        MethodId = a.MethodId,
                        EstimatedSuccessRate = Math.Round(a.SuccessRate * 100, 2),
                        Rank = i + 1
                    }).ToList();

                var best = ranked.First();

                return new RoutingDecision
                {
                    RecommendedMethodId = best.MethodId,
                    Ranking = ranked,
                    Reason = $"UCB1 selected {best.MethodId} " +
                             $"(success rate: {best.EstimatedSuccessRate}%, " +
                             $"context: {context.ContextKey})"
                };
            }
        }

        public void RecordOutcome(BanditContext context, string methodId, bool success)
        {
            lock (_lock)
            {
                var arms = GetOrCreateArms(context.ContextKey);
                if (!arms.TryGetValue(methodId, out var arm)) return;

                arm.TotalAttempts++;
                if (success) arm.SuccessCount++;
            }
        }

        public Dictionary<string, ArmStats> GetStats(BanditContext context)
        {
            lock (_lock)
            {
                var arms = GetOrCreateArms(context.ContextKey);
                return arms.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new ArmStats
                    {
                        MethodId = kvp.Value.MethodId,
                        SuccessCount = kvp.Value.SuccessCount,
                        TotalAttempts = kvp.Value.TotalAttempts,
                        SuccessRate = Math.Round(kvp.Value.SuccessRate * 100, 2),
                        UCBScore = Math.Round(kvp.Value.UCBScore(_totalRounds), 4)
                    }
                );
            }
        }
        private Dictionary<string, BanditArm> GetOrCreateArms(string contextKey)
        {
            if(!_contextArms.TryGetValue(contextKey, out var arms))
            {
                arms = _methodIds.ToDictionary(
                        id => id,
                        id => new BanditArm {   MethodId = id   }
                    );
                _contextArms[contextKey] = arms;
            }
            return arms;
        }
    }
}

public class ArmStats
{
    public string MethodId { get; init; } = default!;
    public int TotalAttempts { get; init; }
    public int SuccessCount { get; init; }
    public double SuccessRate { get; init; }
    public double UCBScore { get; init; }
}
