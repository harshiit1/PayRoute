namespace PayRoute.Bandit.Models
{
    public class BanditContext
    {
        public string UserSegment { get; init; }         // "NEW", "RETURNING", "VIP"
        public string OrderValueBucket { get; init; }    // "LOW", "MID", "HIGH"
        public int HourOfDay { get; init; }
        public string ContextKey => $"{UserSegment}_{OrderValueBucket}_{HourBucket}";

        private string HourBucket => HourOfDay switch
        {
            >= 6 and < 12 => "MORNING",
            >= 12 and < 17 => "AFTERNOON",
            >= 17 and < 21 => "EVENING",
            _ => "NIGHT"
        };
    }
}
