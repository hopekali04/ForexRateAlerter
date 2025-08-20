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
                var apiKey = _configuration["ExchangeRateApi:ApiKey"];
                var baseUrl = _configuration["ExchangeRateApi:BaseUrl"];

                foreach (var baseCurrency in _supportedCurrencies)
                {
                    var url = $"{baseUrl}/latest/{baseCurrency}?access_key={apiKey}";
                    var response = await _httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Failed to fetch rates for {baseCurrency}: {response.StatusCode}");
                        continue;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content);

                    if (apiResponse?.Success == true && apiResponse.Rates != null)
                    {
                        var exchangeRates = new List<ExchangeRate>();

                        foreach (var rate in apiResponse.Rates)
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

        private class ExchangeRateApiResponse
        {
            public bool Success { get; set; }
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }
    }
}