namespace Backend.Infrastructure
{
    public class YookassaSettings
    {
        public string ShopId { get; set; } = string.Empty;
        public string SecretKey { get; set;} = string.Empty;
        public string ReturnUrl {  get; set; } = string.Empty;
        public string WebhookUrl {  get; set; } = string.Empty;
        public bool TestMode {  get; set; } = true;

    }
}
