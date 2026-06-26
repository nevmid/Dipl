public class YooKassaPaymentResponse
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Paid { get; set; }
    public Amount Amount { get; set; } = new();
    public ConfirmationDetails Confirmation { get; set; } = new();
    public string Created_at { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ConfirmationDetails
{
    public string Type { get; set; } = string.Empty;
    public string Confirmation_url { get; set; } = string.Empty;
    public string Return_url { get; set; } = string.Empty;
}
