namespace ForexRateAlerter.Core.Models
{
    /// <summary>
    /// Historical exchange rate snapshots collected hourly for trend analysis
    /// </summary>
    public class ExchangeRateHistory
    {
        public int Id { get; set; }
        
        public string BaseCurrency { get; set; } = string.Empty;
        
        public string TargetCurrency { get; set; } = string.Empty;
        
        public decimal Rate { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string Source { get; set; } = string.Empty; // API source name
    }
}
