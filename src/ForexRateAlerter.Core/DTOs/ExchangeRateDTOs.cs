using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Core.DTOs
{
    public class ExchangeRateDto
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class OhlcDataResponse
    {
        public IEnumerable<OHLCData> Candles { get; set; } = Enumerable.Empty<OHLCData>();
        public string Timeframe { get; set; } = string.Empty;
        public int Count { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class EnrichedExchangeRateDto
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
        public decimal High24h { get; set; }
        public decimal Low24h { get; set; }
        public decimal Open24h { get; set; }
        public decimal Change24h { get; set; }
    }
}
