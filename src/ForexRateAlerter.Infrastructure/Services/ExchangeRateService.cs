using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Core.DTOs;
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
            // Load all rates into memory first, then group (exchange rates dataset is manageable)
            var allRates = await _context.ExchangeRates.ToListAsync();
            
            var latestRates = allRates
                .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                .Select(g => g.OrderByDescending(r => r.Timestamp).First())
                .ToList();

            return latestRates;
        }

        public async Task<IEnumerable<EnrichedExchangeRateDto>> GetEnrichedRatesAsync()
        {
            var latestRates = await GetLatestRatesAsync();
            var fromDate = DateTime.UtcNow.AddHours(-24);

            // Fetch 24h stats using grouping - materialize the groups first to safely access properties
            var allRatesInWindow = await _context.ExchangeRates
                .Where(r => r.Timestamp >= fromDate)
                .ToListAsync();

            var stats = allRatesInWindow
                .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                .Where(g => g.Any()) // Ensure group is not empty
                .Select(g => new
                {
                    g.Key.BaseCurrency,
                    g.Key.TargetCurrency,
                    High = g.Max(r => r.Rate),
                    Low = g.Min(r => r.Rate),
                    // Get the first rate in the 24h window for the "Open" price
                    Open = g.OrderBy(r => r.Timestamp).FirstOrDefault()?.Rate ?? 0
                })
                .Where(s => s.Open != 0) // Filter out invalid data
                .ToList();

            return latestRates.Select(rate =>
            {
                var s = stats.FirstOrDefault(x => x.BaseCurrency == rate.BaseCurrency && x.TargetCurrency == rate.TargetCurrency);
                return new EnrichedExchangeRateDto
                {
                    BaseCurrency = rate.BaseCurrency,
                    TargetCurrency = rate.TargetCurrency,
                    Rate = rate.Rate,
                    Timestamp = rate.Timestamp,
                    Source = rate.Source,
                    High24h = s?.High ?? rate.Rate,
                    Low24h = s?.Low ?? rate.Rate,
                    Open24h = s?.Open ?? rate.Rate,
                    Change24h = (s != null && s.Open != 0) ? ((rate.Rate - s.Open) / s.Open) * 100 : 0
                };
            });
        }

        public async Task<IEnumerable<ExchangeRate>> GetAllRatesAsync()
        {
            // Returns all latest rates for historical collection
            return await GetLatestRatesAsync();
        }

        public async Task<bool> FetchAndStoreLatestRatesAsync()
        {
            try
            {
                var apiKey = _configuration["ExchangeRateApi:ApiKey"]?.Trim();
                var baseUrl = _configuration["ExchangeRateApi:BaseUrl"]?.Trim();

                // 1. Get existing latest rates to compare against (Change Detection)
                var existingRates = (await GetLatestRatesAsync())
                    .ToDictionary(r => $"{r.BaseCurrency}-{r.TargetCurrency}", r => r.Rate);
                
                bool anyChanges = false;
                var timestamp = DateTime.UtcNow;

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

                    if (apiResponse?.Result == "success" && apiResponse.ConversionRates != null)
                    {
                        _logger.LogInformation(
                            "API Response for {Base}: Result={Result}, Rates Count={Count}, Time Last Update UTC={TimeLastUpdate}",
                            baseCurrency,
                            apiResponse.Result,
                            apiResponse.ConversionRates?.Count ?? 0,
                            apiResponse.TimeLastUpdateUtc ?? "N/A"
                        );

                        var newRates = new List<ExchangeRate>();
                        var historyRecords = new List<ExchangeRateHistory>();

                        foreach (var rate in apiResponse.ConversionRates)
                        {
                            if (_supportedCurrencies.Contains(rate.Key) && rate.Key != baseCurrency)
                            {
                                var key = $"{baseCurrency}-{rate.Key}";
                                var currentRateValue = rate.Value;
                                bool isChanged = true;

                                // Check if rate has changed
                                if (existingRates.TryGetValue(key, out var existingRateVal))
                                {
                                    if (existingRateVal == currentRateValue)
                                    {
                                        isChanged = false;
                                    }
                                }

                                if (isChanged)
                                {
                                    // Add to ExchangeRates (Current State/Log)
                                    newRates.Add(new ExchangeRate
                                    {
                                        BaseCurrency = baseCurrency,
                                        TargetCurrency = rate.Key,
                                        Rate = currentRateValue,
                                        Source = "ExchangeRate-API",
                                        Timestamp = timestamp
                                    });

                                    // Add to ExchangeRateHistory (Historical Analysis)
                                    historyRecords.Add(new ExchangeRateHistory
                                    {
                                        BaseCurrency = baseCurrency,
                                        TargetCurrency = rate.Key,
                                        Rate = currentRateValue,
                                        CreatedAt = timestamp,
                                        Source = "ExchangeRate-API"
                                    });
                                }
                            }
                        }

                        if (newRates.Any())
                        {
                            _context.ExchangeRates.AddRange(newRates);
                            _context.ExchangeRateHistory.AddRange(historyRecords);
                            anyChanges = true;
                            _logger.LogInformation($"Detected {newRates.Count} rate changes for base {baseCurrency}");
                        }
                    }
                }

                if (anyChanges)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully fetched and stored updated exchange rates at {timestamp}");
                }
                else
                {
                    _logger.LogInformation("No exchange rate changes detected. DB update skipped.");
                }

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
                .Take(limit * 2) // Limit raw records to a reasonable multiple of the requested limit
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
            // Parse formats like "1m", "5m", "15m", "1h", "1D", or just "D" (defaults to 1)
            if (string.IsNullOrWhiteSpace(timeframe))
            {
                throw new ArgumentException("Timeframe cannot be null or empty.", nameof(timeframe));
            }

            // Regex pattern: optional digits followed by required letter(s)
            var match = Regex.Match(timeframe.Trim(), @"^(\d*)([a-zA-Z]+)$");
            
            if (!match.Success)
            {
                throw new ArgumentException(
                    $"Invalid timeframe format '{timeframe}'. Expected format: [number]unit (e.g., '1h', '5m', '1D', or 'D').", 
                    nameof(timeframe));
            }

            // Extract number (default to 1 if not provided)
            var numberStr = match.Groups[1].Value;
            var interval = string.IsNullOrEmpty(numberStr) ? 1 : int.Parse(numberStr);
            
            // Extract and validate unit
            var unit = match.Groups[2].Value.ToLower();
            
            // Validate supported timeframe units
            if (!new[] { "m", "h", "d" }.Contains(unit))
            {
                throw new ArgumentException(
                    $"Unsupported timeframe unit '{unit}'. Supported units: m (minutes), h (hours), d (days).", 
                    nameof(timeframe));
            }

            if (interval <= 0)
            {
                throw new ArgumentException(
                    $"Timeframe interval must be positive. Got: {interval}", 
                    nameof(timeframe));
            }
            
            return (interval, unit);
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

        [System.Obsolete("This method is deprecated - use IExchangeRateHistoryService.GetTopMoversAsync instead")]
        public Task<TopMoversResponse> GetTopMoversAsync(string timeframe = "24h")
        {
            // This method is deprecated - use IExchangeRateHistoryService instead
            return Task.FromResult(new TopMoversResponse
            {
                TopMovers = Enumerable.Empty<TopMoverDto>(),
                Timeframe = timeframe,
                GeneratedAt = DateTime.UtcNow
            });
        }

        private class ExchangeRateApiResponse
        {
            public string? Result { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("conversion_rates")]
            public Dictionary<string, decimal> ConversionRates { get; set; } = new();
            [System.Text.Json.Serialization.JsonPropertyName("time_last_update_utc")]
            public string? TimeLastUpdateUtc { get; set; }
        }
    }
}