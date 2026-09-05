# PayRoute — Intelligent Payment Method Optimizer

> A contextual multi-armed bandit engine that learns which payment method succeeds best for each user segment, order value, and time of day — built for the Razorpay AI Buildathon.

---

## The Problem

Payment failures cost money. A VIP customer placing a ₹50,000 order at 2 PM has a different optimal payment method than a new user placing a ₹200 order at midnight. Static routing ignores this. PayRoute learns it.

---

## How It Works

PayRoute uses the **UCB1 (Upper Confidence Bound)** algorithm — a contextual multi-armed bandit — to dynamically route each payment attempt to the method most likely to succeed given:

- **User Segment** — NEW, RETURNING, VIP
- **Order Value Bucket** — LOW (<₹500), MID (₹500–₹5000), HIGH (>₹5000)
- **Time of Day** — MORNING, AFTERNOON, EVENING, NIGHT

Each context bucket maintains independent success statistics per payment method. UCB1 balances exploration (trying untested methods) with exploitation (routing to proven winners). Over time it converges to the highest-success method per context.

---

## Architecture
PayRoute.Domain — Core entities: PaymentMethod, PaymentAttempt, RoutingDecision
PayRoute.Bandit — UCB1 engine: BanditArm, BanditContext, PaymentBanditEngine
PayRoute.Application — Application layer (extensible)
PayRoute.API — ASP.NET Core REST API 


**Key design decisions:**
- Engine registered as a **singleton** — state persists across requests, enabling real learning
- **Thread-safe** via lock — safe for concurrent payment requests
- Context bucketing separates learning per segment — VIP+HIGH learns independently from NEW+LOW
- No database dependency — in-memory state, zero infrastructure to run

---

## API Endpoints

### `POST /api/route`
Get the recommended payment method for a given context.

**Request:**
```json
{
  "userSegment": "VIP",
  "orderValueBucket": "HIGH",
  "hourOfDay": 14
}
```

**Response:**
```json
{
  "recommendedMethodId": "upi",
  "reason": "UCB1 selected upi (success rate: 92%, context: VIP_HIGH_AFTERNOON)",
  "ranking": [
    { "methodId": "upi", "estimatedSuccessRate": 92, "rank": 1 },
    { "methodId": "card_visa", "estimatedSuccessRate": 85, "rank": 2 },
    { "methodId": "card_mastercard", "estimatedSuccessRate": 83, "rank": 3 },
    { "methodId": "netbanking", "estimatedSuccessRate": 61, "rank": 4 },
    { "methodId": "wallet_paytm", "estimatedSuccessRate": 74, "rank": 5 },
    { "methodId": "bnpl", "estimatedSuccessRate": 68, "rank": 6 }
  ]
}
```

---

### `POST /api/outcome`
Record whether a payment attempt succeeded or failed. This is how the engine learns.

**Request:**
```json
{
  "userSegment": "VIP",
  "orderValueBucket": "HIGH",
  "hourOfDay": 14,
  "methodId": "upi",
  "success": true
}
```

**Response:**
```json
{ "message": "Outcome recorded." }
```

---

### `GET /api/stats`
Inspect current bandit state for a context — success rates, attempt counts, UCB scores.

**Query params:** `userSegment`, `orderValueBucket`, `hourOfDay`

**Example:** `GET /api/stats?userSegment=VIP&orderValueBucket=HIGH&hourOfDay=14`

**Response:**
```json
{
  "upi": {
    "methodId": "upi",
    "successCount": 460,
    "totalAttempts": 500,
    "successRate": 92.0,
    "ucbScore": 0.9214
  }
}
```

---

## Running Locally

**Prerequisites:** .NET 10 SDK

```bash
git clone https://github.com/harshiit1/PayRoute
cd PayRoute
dotnet run --project PayRoute.API
```

Open Swagger UI: `http://localhost:5072/swagger`

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10, Swagger/OpenAPI |
| Algorithm | UCB1 Contextual Multi-Armed Bandit |
| Language | C# 13 |
| Architecture | Clean Architecture, Singleton Engine |
| Threading | Lock-based thread safety |

---

## Why UCB1

UCB1 is provably optimal for the explore-exploit tradeoff. It doesn't require training data upfront — it learns from live outcomes. This makes it production-deployable from day one, unlike ML models that need historical datasets before they're useful.
For a payment routing problem where failure rates shift with time, user behavior, and gateway availability, UCB1 is a better fit than a static model.
