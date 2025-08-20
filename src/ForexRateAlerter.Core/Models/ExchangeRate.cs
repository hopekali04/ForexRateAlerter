namespace ForexRateAlerter.Core.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        
        public string BaseCurrency { get; set; } = string.Empty;
        
        public string TargetCurrency { get; set; } = string.Empty;
        
        public decimal Rate { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string Source { get; set; } = string.Empty; // API source name
    }
}
