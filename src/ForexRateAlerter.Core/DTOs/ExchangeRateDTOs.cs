using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Core.DTOs
{
    public class OhlcDataResponse
    {
        public IEnumerable<OHLCData> Candles { get; set; } = Enumerable.Empty<OHLCData>();
        public string Timeframe { get; set; } = string.Empty;
        public int Count { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
