namespace ForexRateAlerter.Core.DTOs
{
    public class TopMoverDto
    {
        public string Pair { get; set; } = string.Empty;
        public decimal LatestRate { get; set; }
        public decimal OldestRate { get; set; }
        public decimal ChangePercent { get; set; }
        public string Direction { get; set; } = string.Empty;
    }

    public class TopMoversResponse
    {
        public IEnumerable<TopMoverDto> TopMovers { get; set; } = Enumerable.Empty<TopMoverDto>();
        public string Timeframe { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
