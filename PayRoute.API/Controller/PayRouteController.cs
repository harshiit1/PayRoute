using Microsoft.AspNetCore.Mvc;
using PayRoute.API.Dtos;
using PayRoute.API.Dtos.PayRoute.API.Dtos;
using PayRoute.Bandit.Engine;
using PayRoute.Bandit.Models;

namespace PayRoute.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PayRouteController : ControllerBase
    {
        private readonly PaymentBanditEngine _engine;

        public PayRouteController(PaymentBanditEngine engine)
        {
            _engine = engine;
        }

        // POST /api/route
        [HttpPost("route")]
        public IActionResult Route([FromBody] RouteRequest request)
        {
            var context = new BanditContext
            {
                UserSegment = request.UserSegment,
                OrderValueBucket = request.OrderValueBucket,
                HourOfDay = request.HourOfDay
            };

            var decision = _engine.Recommend(context);

            return Ok(new RouteResponse
            {
                RecommendedMethodId = decision.RecommendedMethodId,
                Reason = decision.Reason,
                Ranking = decision.Ranking.Select(r => new RankedMethodDto
                {
                    MethodId = r.MethodId,
                    EstimatedSuccessRate = r.EstimatedSuccessRate,
                    Rank = r.Rank
                }).ToList()
            });
        }

        // POST /api/outcome
        [HttpPost("outcome")]
        public IActionResult Outcome([FromBody] OutcomeRequest request)
        {
            var context = new BanditContext
            {
                UserSegment = request.UserSegment,
                OrderValueBucket = request.OrderValueBucket,
                HourOfDay = request.HourOfDay
            };

            _engine.RecordOutcome(context, request.MethodId, request.Success);
            return Ok(new { message = "Outcome recorded." });
        }

        // GET /api/stats
        [HttpGet("stats")]
        public IActionResult Stats(
          [FromQuery] string userSegment = "RETURNING",
          [FromQuery] string orderValueBucket = "MID",
          [FromQuery] int hourOfDay = 12)
        {
            var context = new BanditContext
            {
                UserSegment = userSegment,
                OrderValueBucket = orderValueBucket,
                HourOfDay = hourOfDay
            };

            var stats = _engine.GetStats(context);
            return Ok(stats);
        }
    }
}