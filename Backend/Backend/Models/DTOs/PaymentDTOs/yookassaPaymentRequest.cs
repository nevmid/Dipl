public class YooKassaPaymentRequest
{
    public Amount Amount { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public string Capture { get; set; } = "true";
    public Confirmation Confirmation { get; set; } = new();
    public Metadata Metadata { get; set; } = new();
}

public class Confirmation
{
    public string Type { get; set; } = "redirect";
    public string Return_url { get; set; } = string.Empty;
}

public class Amount
{
    public string Value { get; set; }
    public string Currency { get; set; } = "RUB";
}