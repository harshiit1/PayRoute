namespace PayRoute.API.Dtos
{
    namespace PayRoute.API.Dtos
    {
        public class RouteRequest
        {
            public string UserSegment { get; set; } = "RETURNING";
            public string OrderValueBucket { get; set; } = "MID";  // LOW | MID | HIGH
            public int HourOfDay { get; set; } = DateTime.UtcNow.Hour;
        }

        public class RouteResponse
        {
            public string RecommendedMethodId { get; set; } = default!;
            public string Reason { get; set; } = default!;
            public List<RankedMethodDto> Ranking { get; set; } = new();
        }

        public class RankedMethodDto
        {
            public string MethodId { get; set; } = default!;
            public double EstimatedSuccessRate { get; set; }
            public int Rank { get; set; }
        }

        public class OutcomeRequest
        {
            public string UserSegment { get; set; } = "RETURNING";
            public string OrderValueBucket { get; set; } = "MID";
            public int HourOfDay { get; set; } = DateTime.UtcNow.Hour;
            public string MethodId { get; set; } = default!;
            public bool Success { get; set; }
        }
    }
}
