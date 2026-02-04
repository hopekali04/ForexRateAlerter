using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ForexRateAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IExchangeRateHistoryService _exchangeRateHistoryService;
        private readonly ILogger<ExchangeRateController> _logger;
        private readonly ApplicationDbContext _context;
        private static readonly HashSet<string> ValidTimeframes = new() { "1m", "5m", "15m", "1h", "1D" };
        private static readonly HashSet<string> TopMoverTimeframes = new() { "24h", "7d", "30d" };

        public ExchangeRateController(
            IExchangeRateService exchangeRateService,
            IExchangeRateHistoryService exchangeRateHistoryService,
            ILogger<ExchangeRateController> logger,
            ApplicationDbContext context)
        {
            _exchangeRateService = exchangeRateService;
            _exchangeRateHistoryService = exchangeRateHistoryService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get latest exchange rates for all supported currency pairs
        /// </summary>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates()
        {
            try
            {
                var rates = await _exchangeRateService.GetLatestRatesAsync();
                return Ok(new { rates, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve latest rates");
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving latest rates." });
            }
        }

        /// <summary>
        /// Get latest exchange rates enriched with 24h statistics
        /// </summary>
        [HttpGet("latest-enriched")]
        public async Task<IActionResult> GetEnrichedRates()
        {
            try
            {
                var rates = await _exchangeRateService.GetEnrichedRatesAsync();
                return Ok(new { rates, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve enriched rates");
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving enriched rates." });
            }
        }

        /// <summary>
        /// Get latest rate for a specific currency pair
        /// </summary>
        [HttpGet("latest/{baseCurrency}/{targetCurrency}")]
        public async Task<IActionResult> GetLatestRate(string baseCurrency, string targetCurrency)
        {
            var rate = await _exchangeRateService.GetLatestRateAsync(
                baseCurrency.ToUpper(), targetCurrency.ToUpper());

            if (rate == null)
                return NotFound(new { error = "Exchange rate not found for the specified currency pair" });

            return Ok(rate);
        }

        /// <summary>
        /// Get rate history for a specific currency pair
        /// </summary>
        [HttpGet("history/{baseCurrency}/{targetCurrency}")]
        public async Task<IActionResult> GetRateHistory(string baseCurrency, string targetCurrency, 
            [FromQuery] int days = 30)
        {
            if (days < 1)
                return BadRequest(new { error = "Days must be between 1 and 365." });
            if (days > 365) days = 365; // Limit to 1 year

            try
            {
                // Use the new history service for accurate historical data
                var history = await _exchangeRateHistoryService.GetHistoricalRatesAsync(
                    baseCurrency.ToUpper(), 
                    targetCurrency.ToUpper(), 
                    days);

                if (!history.Any())
                {
                    return Ok(new 
                    { 
                        history = Array.Empty<object>(),
                        message = "No historical data available yet. Data collection in progress.",
                        days
                    });
                }

                return Ok(new { history, days });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve rate history for {Base}/{Target}", baseCurrency, targetCurrency);
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving rate history." });
            }
        }

        /// <summary>
        /// Get OHLC (candlestick) data for charting
        /// </summary>
        [HttpGet("ohlc/{baseCurrency}/{targetCurrency}")]
        public async Task<IActionResult> GetOHLCData(string baseCurrency, string targetCurrency,
            [FromQuery] string timeframe = "1h", [FromQuery] int limit = 100)
        {
            // Validate timeframe
            if (!ValidTimeframes.Contains(timeframe))
                return BadRequest(new { error = "Invalid timeframe. Allowed: 1m, 5m, 15m, 1h, 1D" });

            // Validate limit
            if (limit < 1 || limit > 1000)
                return BadRequest(new { error = "Limit must be between 1 and 1000" });

            var candles = await _exchangeRateService.GetOHLCDataAsync(
                baseCurrency.ToUpper(), targetCurrency.ToUpper(), timeframe, limit);

            var response = new OhlcDataResponse
            {
                Candles = candles,
                Timeframe = timeframe,
                Count = candles.Count(),
                BaseCurrency = baseCurrency.ToUpper(),
                TargetCurrency = targetCurrency.ToUpper(),
                Timestamp = DateTime.UtcNow
            };

            return Ok(response);
        }

        /// <summary>
        /// Get top currency movers for a specific timeframe
        /// </summary>
        /// <param name="timeframe">24h, 7d, or 30d</param>
        /// <param name="limit">Number of top movers to return (default 5)</param>
        [HttpGet("top-movers")]
        public async Task<IActionResult> GetTopMovers([FromQuery] string timeframe = "24h", [FromQuery] int limit = 5)
        {
            if (!TopMoverTimeframes.Contains(timeframe.ToLower()))
            {
                return BadRequest(new { error = $"Invalid timeframe. Must be one of: {string.Join(", ", TopMoverTimeframes)}" });
            }

            if (limit < 1 || limit > 20)
            {
                return BadRequest(new { error = "Limit must be between 1 and 20" });
            }

            try
            {
                var topMovers = await _exchangeRateHistoryService.GetTopMoversAsync(timeframe, limit);
                
                if (!topMovers.Any())
                {
                    return Ok(new 
                    { 
                        topMovers = Array.Empty<object>(),
                        timeframe,
                        message = "No historical data available yet. Data collection started. Check back in 24 hours."
                    });
                }

                return Ok(new { topMovers, timeframe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve top movers for timeframe {Timeframe}", timeframe);
                 return StatusCode(500, new { error = "An unexpected error occurred while retrieving top movers." });  
            }
        }

        /// <summary>
        /// Manually trigger exchange rate update (Admin only)
        /// </summary>
        [HttpPost("refresh")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefreshRates()
        {
            var success = await _exchangeRateService.FetchAndStoreLatestRatesAsync();
            
            if (success)
                return Ok(new { message = "Exchange rates updated successfully" });
            
            return StatusCode(500, new { error = "Failed to update exchange rates" });
        }

        /// <summary>
        /// DEBUG: Check RAW data for CAD/AUD pair in last 24h
        /// </summary>
        [HttpGet("debug/raw-cad-aud")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugRawCadAud()
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddHours(-24);
                
                // Get ALL CAD/AUD records from last 24h
                var cadAudRecords = await _context.ExchangeRates
                    .Where(r => r.BaseCurrency == "CAD" && r.TargetCurrency == "AUD" && r.Timestamp >= fromDate)
                    .OrderBy(r => r.Timestamp)
                    .Select(r => new
                    {
                        r.Rate,
                        r.Timestamp,
                        r.Source,
                        AgeMinutes = (DateTime.UtcNow - r.Timestamp).TotalMinutes
                    })
                    .ToListAsync();

                var stats = cadAudRecords.Any() ? new
                {
                    count = cadAudRecords.Count,
                    min = cadAudRecords.Min(r => r.Rate),
                    max = cadAudRecords.Max(r => r.Rate),
                    oldest = cadAudRecords.First().Rate,
                    latest = cadAudRecords.Last().Rate,
                    uniqueRates = cadAudRecords.Select(r => r.Rate).Distinct().Count(),
                    calculatedChange = cadAudRecords.Any() && cadAudRecords.First().Rate != 0
                        ? ((cadAudRecords.Last().Rate - cadAudRecords.First().Rate) / cadAudRecords.First().Rate) * 100
                        : 0
                } : null;

                return Ok(new
                {
                    pair = "CAD/AUD",
                    currentTime = DateTime.UtcNow,
                    lookbackTime = fromDate,
                    recordCount = cadAudRecords.Count,
                    records = cadAudRecords,
                    statistics = stats,
                    diagnosis = stats?.uniqueRates > 1 
                        ? $"✅ Found {stats.uniqueRates} unique rates - variation exists!"
                        : "❌ All records have identical rates"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug raw data check failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// DEBUG: Check timestamps of data in ExchangeRates table
        /// </summary>
        [HttpGet("debug/data-freshness")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugDataFreshness()
        {
            try
            {
                var now = DateTime.UtcNow;
                
                // Get timestamp range from ExchangeRates table
                var exchangeRates = await _context.ExchangeRates
                    .OrderByDescending(r => r.Timestamp)
                    .Take(10)
                    .Select(r => new
                    {
                        r.BaseCurrency,
                        r.TargetCurrency,
                        r.Rate,
                        r.Timestamp,
                        r.Source,
                        AgeMinutes = (now - r.Timestamp).TotalMinutes
                    })
                    .ToListAsync();

                // Check 24h window for any rate changes
                var last24h = await _context.ExchangeRates
                    .Where(r => r.Timestamp >= now.AddHours(-24))
                    .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                    .Select(g => new
                    {
                        Pair = $"{g.Key.BaseCurrency}/{g.Key.TargetCurrency}",
                        Count = g.Count(),
                        MinRate = g.Min(r => r.Rate),
                        MaxRate = g.Max(r => r.Rate),
                        HasVariation = g.Min(r => r.Rate) != g.Max(r => r.Rate),
                        OldestTimestamp = g.Min(r => r.Timestamp),
                        LatestTimestamp = g.Max(r => r.Timestamp)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    currentTime = now,
                    latestRecords = exchangeRates,
                    last24hAnalysis = new
                    {
                        totalPairs = last24h.Count,
                        pairsWithVariation = last24h.Count(p => p.HasVariation),
                        pairsWithoutVariation = last24h.Count(p => !p.HasVariation),
                        samplePairs = last24h.Take(5)
                    },
                    diagnosis = last24h.Any(p => p.HasVariation)
                        ? $"✅ {last24h.Count(p => p.HasVariation)} pairs show rate variation in last 24h"
                        : "❌ NO rate variation detected - API returning same rates repeatedly"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug freshness check failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// DEBUG: Inspect ExchangeRateHistory table for all pairs - check which ones have movement
        /// </summary>
        [HttpGet("debug/all-history-data")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugAllHistoryData()
        {
            try
            {
                var lookbackTime = DateTime.UtcNow.AddHours(-24);
                
                // Get ALL historical data from last 24h
                var allHistory = await _context.ExchangeRates
                    .Where(r => r.Timestamp >= lookbackTime)
                    .OrderBy(r => r.BaseCurrency)
                    .ThenBy(r => r.TargetCurrency)
                    .ThenBy(r => r.Timestamp)
                    .ToListAsync();

                var analysis = allHistory
                    .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                    .Select(g => new
                    {
                        pair = $"{g.Key.BaseCurrency}/{g.Key.TargetCurrency}",
                        recordCount = g.Count(),
                        oldestRate = g.OrderBy(r => r.Timestamp).First().Rate,
                        latestRate = g.OrderByDescending(r => r.Timestamp).First().Rate,
                        changePercent = g.OrderBy(r => r.Timestamp).First().Rate != 0
                            ? ((g.OrderByDescending(r => r.Timestamp).First().Rate - g.OrderBy(r => r.Timestamp).First().Rate) 
                               / g.OrderBy(r => r.Timestamp).First().Rate) * 100
                            : 0,
                        oldestTimestamp = g.OrderBy(r => r.Timestamp).First().Timestamp,
                        latestTimestamp = g.OrderByDescending(r => r.Timestamp).First().Timestamp,
                        hasMovement = g.OrderBy(r => r.Timestamp).First().Rate != g.OrderByDescending(r => r.Timestamp).First().Rate
                    })
                    .OrderByDescending(x => Math.Abs(x.changePercent))
                    .ToList();

                var pairsWithMovement = analysis.Where(x => x.hasMovement).ToList();
                var pairsWithoutMovement = analysis.Where(x => !x.hasMovement).ToList();

                return Ok(new
                {
                    currentTime = DateTime.UtcNow,
                    lookbackTime = lookbackTime,
                    totalPairs = analysis.Count,
                    pairsWithMovement = pairsWithMovement.Count,
                    pairsWithoutMovement = pairsWithoutMovement.Count,
                    topMovers = pairsWithMovement.Take(10),
                    staticPairs = pairsWithoutMovement.Take(5),
                    diagnosis = pairsWithMovement.Any() 
                        ? $"✅ {pairsWithMovement.Count} pairs have movement - top-movers should work!" 
                        : "❌ NO pairs have movement - market might be closed or API returning stale data"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Debug endpoint failed");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
