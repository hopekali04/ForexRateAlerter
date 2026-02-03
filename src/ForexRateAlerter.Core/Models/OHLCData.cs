namespace ForexRateAlerter.Core.Models
{
    public class OHLCData
    {
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
    }
}
