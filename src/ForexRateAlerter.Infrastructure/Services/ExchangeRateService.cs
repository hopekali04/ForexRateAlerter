using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExchangeRateService> _logger;

        private readonly string[] _supportedCurrencies = { "USD", "EUR", "GBP", "MWK", "ZAR", "JPY", "CAD", "AUD" };

        public ExchangeRateService(ApplicationDbContext context, HttpClient httpClient, 
            IConfiguration configuration, ILogger<ExchangeRateService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExchangeRate?> GetLatestRateAsync(string baseCurrency, string targetCurrency)
        {
            return await _context.ExchangeRates
                .Where(r => r.BaseCurrency == baseCurrency && r.TargetCurrency == targetCurrency)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync()
        {
            var latestRates = await _context.ExchangeRates
                .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                .Select(g => g.OrderByDescending(r => r.Timestamp).First())
                .ToListAsync();

            return latestRates;
        }

        public async Task<bool> FetchAndStoreLatestRatesAsync()
        {
            try
            {
                var apiKey = _configuration["ExchangeRateApi:ApiKey"]?.Trim();
                var baseUrl = _configuration["ExchangeRateApi:BaseUrl"]?.Trim();

                foreach (var baseCurrency in _supportedCurrencies)
                {
                    var url = $"{baseUrl}/{apiKey}/latest/{baseCurrency}";
                    var response = await _httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Failed to fetch rates for {baseCurrency}: {response.StatusCode}");
                        continue;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                    if (apiResponse?.Result == "success" && apiResponse.Conversion_rates != null)
                    {
                        var exchangeRates = new List<ExchangeRate>();

                        foreach (var rate in apiResponse.Conversion_rates)
                        {
                            if (_supportedCurrencies.Contains(rate.Key) && rate.Key != baseCurrency)
                            {
                                exchangeRates.Add(new ExchangeRate
                                {
                                    BaseCurrency = baseCurrency,
                                    TargetCurrency = rate.Key,
                                    Rate = rate.Value,
                                    Source = "ExchangeRate-API",
                                    Timestamp = DateTime.UtcNow
                                });
                            }
                        }

                        _context.ExchangeRates.AddRange(exchangeRates);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully fetched and stored exchange rates at {DateTime.UtcNow}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rates");
                return false;
            }
        }

        public async Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(string baseCurrency, string targetCurrency, int days = 30)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            return await _context.ExchangeRates
                .Where(r => r.BaseCurrency == baseCurrency && 
                           r.TargetCurrency == targetCurrency && 
                           r.Timestamp >= fromDate)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<OHLCData>> GetOHLCDataAsync(string baseCurrency, string targetCurrency, string timeframe = "1h", int limit = 100)
        {
            // Parse timeframe (1m, 5m, 15m, 1h, 1D)
            var (interval, intervalType) = ParseTimeframe(timeframe);
            
            // Calculate how far back to fetch data
            var daysToFetch = CalculateDaysToFetch(interval, intervalType, limit);
            var fromDate = DateTime.UtcNow.AddDays(-daysToFetch);

            // Fetch raw rates
            var rates = await _context.ExchangeRates
                .Where(r => r.BaseCurrency == baseCurrency && 
                           r.TargetCurrency == targetCurrency && 
                           r.Timestamp >= fromDate)
                .OrderBy(r => r.Timestamp)
                .ToListAsync();

            if (!rates.Any())
                return new List<OHLCData>();

            // Group by timeframe and aggregate to OHLC
            var ohlcData = new List<OHLCData>();
            
            foreach (var group in GroupByTimeframe(rates, interval, intervalType))
            {
                if (!group.Any()) continue;

                ohlcData.Add(new OHLCData
                {
                    Time = group.Key,
                    Open = group.First().Rate,
                    High = group.Max(r => r.Rate),
                    Low = group.Min(r => r.Rate),
                    Close = group.Last().Rate,
                    BaseCurrency = baseCurrency,
                    TargetCurrency = targetCurrency
                });
            }

            return ohlcData.TakeLast(limit).ToList();
        }

        private (int interval, string type) ParseTimeframe(string timeframe)
        {
            // Parse formats like "1m", "5m", "15m", "1h", "1D"
            var number = int.Parse(new string(timeframe.Where(char.IsDigit).ToArray()));
            var unit = new string(timeframe.Where(char.IsLetter).ToArray()).ToLower();
            
            return (number, unit);
        }

        private int CalculateDaysToFetch(int interval, string intervalType, int limit)
        {
            return intervalType switch
            {
                "m" => Math.Max((interval * limit) / (60 * 24), 1), // Minutes to days
                "h" => Math.Max((interval * limit) / 24, 1),        // Hours to days
                "d" => interval * limit,                             // Days
                _ => 7 // Default to 1 week
            };
        }

        private IEnumerable<IGrouping<DateTime, ExchangeRate>> GroupByTimeframe(
            List<ExchangeRate> rates, int interval, string intervalType)
        {
            return intervalType switch
            {
                "m" => rates.GroupBy(r => new DateTime(
                    r.Timestamp.Year, r.Timestamp.Month, r.Timestamp.Day,
                    r.Timestamp.Hour, (r.Timestamp.Minute / interval) * interval, 0)),
                "h" => rates.GroupBy(r => new DateTime(
                    r.Timestamp.Year, r.Timestamp.Month, r.Timestamp.Day,
                    (r.Timestamp.Hour / interval) * interval, 0, 0)),
                "d" => rates.GroupBy(r => new DateTime(
                    r.Timestamp.Year, r.Timestamp.Month, r.Timestamp.Day)),
                _ => rates.GroupBy(r => r.Timestamp.Date)
            };
        }

        private class ExchangeRateApiResponse
        {
            public string? Result { get; set; }
            public Dictionary<string, decimal> Conversion_rates { get; set; } = new();
        }
    }
}