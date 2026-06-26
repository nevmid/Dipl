public class YooKassaWebhookPayload
{
    public string Type { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public PaymentObject Object { get; set; } = new();
}

public class PaymentObject
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Paid { get; set; }
    public Amount Amount { get; set; } = new();
    public string CreatedAt { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Metadata Metadata { get; set; } = new();
}

public class Metadata
{
    public string BookingId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}