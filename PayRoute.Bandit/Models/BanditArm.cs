namespace PayRoute.Bandit.Models
{
    public class BanditArm
    {
        public string MethodId { get; init; }
        public int TotalAttempts { get; set; } = 0;
        public int SuccessCount { get; set; } = 0;
        public double SuccessRate => TotalAttempts == 0 ? 0.5 : (double)SuccessCount / TotalAttempts;

        public double UCBScore(int totalRounds, double explorationFactor = 1.5)
        {
            if (TotalAttempts == 0) return double.MaxValue;
            return SuccessRate + explorationFactor * Math.Sqrt(Math.Log(totalRounds) / TotalAttempts);
        }
    }
}
