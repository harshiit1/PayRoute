namespace PayRoute.Domain.Entities
{
    public class PaymentMethod
    {
        public string Id { get; init; }              // "UPI", "CARD", "NETBANKING", "WALLET"
        public string DisplayName { get; init; }
        public bool IsActive { get; set; } = true;
    }
}
