namespace ForexRateAlerter.Core.Models
{
    public class ExternalApiSettings
    {
        public string Key { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string[] SupportedCurrencies { get; set; } = Array.Empty<string>();
    }
}
