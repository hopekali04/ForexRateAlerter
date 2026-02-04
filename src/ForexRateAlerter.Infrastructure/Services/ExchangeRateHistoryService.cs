using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services;

public class ExchangeRateHistoryService : IExchangeRateHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ILogger<ExchangeRateHistoryService> _logger;
    private const string ExchangeRateApiSource = "ExchangeRateAPI";

    public ExchangeRateHistoryService(
        ApplicationDbContext context,
        IExchangeRateService exchangeRateService,
        ILogger<ExchangeRateHistoryService> logger)
    {
        _context = context;
        _exchangeRateService = exchangeRateService;
        _logger = logger;
    }

    public async Task StoreCurrentRatesAsync()
    {
        try
        {
            _logger.LogInformation("Starting hourly exchange rate collection...");

            // Fetch all current rates from external API
            var currentRates = await _exchangeRateService.GetAllRatesAsync();

            if (currentRates == null || !currentRates.Any())
            {
                _logger.LogWarning("No rates returned from external API");
                return;
            }

            var timestamp = DateTime.UtcNow;

            // Store each rate in history table
            foreach (var rate in currentRates)
            {
                _context.ExchangeRateHistory.Add(new Core.Models.ExchangeRateHistory
                {
                    BaseCurrency = rate.BaseCurrency,
                    TargetCurrency = rate.TargetCurrency,
                    Rate = rate.Rate,
                    CreatedAt = timestamp,
                    Source = ExchangeRateApiSource
                });
            }

            var savedCount = await _context.SaveChangesAsync();
            _logger.LogInformation($"Stored {savedCount} exchange rate snapshots at {timestamp:yyyy-MM-dd HH:mm:ss} UTC");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store exchange rate history");
            throw;
        }
    }

    public async Task<IEnumerable<TopMoverDto>> GetTopMoversAsync(string timeframe, int limit = 5)
    {
        try
        {
            // Calculate lookback time
            var lookbackTime = timeframe?.ToLower() switch
            {
                "24h" => DateTime.UtcNow.AddHours(-24),
                "7d" => DateTime.UtcNow.AddDays(-7),
                "30d" => DateTime.UtcNow.AddDays(-30),
                _ => DateTime.UtcNow.AddHours(-24)
            };

            _logger.LogInformation($"Fetching top {limit} movers for timeframe: {timeframe} (since {lookbackTime:yyyy-MM-dd HH:mm:ss} UTC)");

            // Query historical data - materialize first to avoid SQL translation issues
            var allHistoricalRates = await _context.ExchangeRateHistory
                .Where(r => r.CreatedAt >= lookbackTime)
                .ToListAsync();

            List<TopMoverDto> topMovers;

            if (allHistoricalRates.Any())
            {
                var ratesInPeriod = allHistoricalRates
                    .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                    .Where(g => g.Count() >= 2) // Need at least 2 data points to calculate change
                    .Select(g => new
                    {
                        Pair = $"{g.Key.BaseCurrency}/{g.Key.TargetCurrency}",
                        LatestRate = g.OrderByDescending(r => r.CreatedAt).First().Rate,
                        OldestRate = g.OrderBy(r => r.CreatedAt).First().Rate,
                        DataPoints = g.Count()
                    })
                    .ToList();

                // Calculate percentage changes
                topMovers = ratesInPeriod
                    .Select(r => new TopMoverDto
                    {
                        Pair = r.Pair,
                        LatestRate = r.LatestRate,
                        OldestRate = r.OldestRate,
                        ChangePercent = r.OldestRate != 0
                            ? ((r.LatestRate - r.OldestRate) / r.OldestRate) * 100
                            : 0,
                        Direction = r.LatestRate > r.OldestRate ? "up" : r.LatestRate < r.OldestRate ? "down" : "unchanged"
                    })
                    .Where(m => m.ChangePercent != 0) // Filter out pairs with 0% change
                    .OrderByDescending(m => Math.Abs(m.ChangePercent))
                    .Take(limit)
                    .ToList();

                // If we have movers, return them
                if (topMovers.Any())
                {
                    _logger.LogInformation($"Found {topMovers.Count} top movers from ExchangeRateHistory");
                    return topMovers;
                }
                
                _logger.LogWarning("ExchangeRateHistory has data but all pairs have 0% change. Falling back to ExchangeRates table.");
            }

            // Fallback: Use ExchangeRates table (more frequent updates)
            _logger.LogInformation("Using ExchangeRates table as fallback for top movers");
            var allRates = await _context.ExchangeRates
                .Where(r => r.Timestamp >= lookbackTime)
                .ToListAsync();

            if (!allRates.Any())
            {
                _logger.LogWarning("No data in ExchangeRates table either");
                return Enumerable.Empty<TopMoverDto>();
            }

            var fallbackRates = allRates
                .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                .Where(g => g.Count() >= 2)
                .Select(g => new
                {
                    Pair = $"{g.Key.BaseCurrency}/{g.Key.TargetCurrency}",
                    LatestRate = g.OrderByDescending(r => r.Timestamp).First().Rate,
                    OldestRate = g.OrderBy(r => r.Timestamp).First().Rate
                })
                .ToList();

            topMovers = fallbackRates
                .Select(r => new TopMoverDto
                {
                    Pair = r.Pair,
                    LatestRate = r.LatestRate,
                    OldestRate = r.OldestRate,
                    ChangePercent = r.OldestRate != 0
                        ? ((r.LatestRate - r.OldestRate) / r.OldestRate) * 100
                        : 0,
                    Direction = r.LatestRate > r.OldestRate ? "up" : r.LatestRate < r.OldestRate ? "down" : "unchanged"
                })
                .Where(m => m.ChangePercent != 0)
                .OrderByDescending(m => Math.Abs(m.ChangePercent))
                .Take(limit)
                .ToList();

            _logger.LogInformation($"Found {topMovers.Count} top movers from ExchangeRates table (fallback)");
            return topMovers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get top movers for timeframe: {timeframe}");
            throw;
        }
    }

    public async Task<IEnumerable<ExchangeRateDto>> GetHistoricalRatesAsync(string baseCurrency, string targetCurrency, int days = 30)
    {
        try
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            var history = await _context.ExchangeRateHistory
                .Where(r => r.BaseCurrency == baseCurrency && 
                           r.TargetCurrency == targetCurrency && 
                           r.CreatedAt >= fromDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return history.Select(h => new ExchangeRateDto
            {
                BaseCurrency = h.BaseCurrency,
                TargetCurrency = h.TargetCurrency,
                Rate = h.Rate,
                Timestamp = h.CreatedAt,
                Source = h.Source
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get historical rates for {BaseCurrency}/{TargetCurrency}", baseCurrency, targetCurrency);
            throw;
        }
    }
}
