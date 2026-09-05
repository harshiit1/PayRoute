using PayRoute.Bandit.Engine;
using PayRoute.Bandit.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register bandit engine as singleton — state must persist across requests
builder.Services.AddSingleton<PaymentBanditEngine>(_ =>
    new PaymentBanditEngine(new[]
    {
        "upi",
        "card_visa",
        "card_mastercard",
        "netbanking",
        "wallet_paytm",
        "bnpl"
    })
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();