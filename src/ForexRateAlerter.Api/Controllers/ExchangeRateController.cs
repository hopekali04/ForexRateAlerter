using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.DTOs;

namespace ForexRateAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IExchangeRateHistoryService _exchangeRateHistoryService;
        private static readonly HashSet<string> ValidTimeframes = new() { "1m", "5m", "15m", "1h", "1D" };

        public ExchangeRateController(
            IExchangeRateService exchangeRateService,
            IExchangeRateHistoryService exchangeRateHistoryService)
        {
            _exchangeRateService = exchangeRateService;
            _exchangeRateHistoryService = exchangeRateHistoryService;
        }

        /// <summary>
        /// Get latest exchange rates for all supported currency pairs
        /// </summary>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates()
        {
            var rates = await _exchangeRateService.GetLatestRatesAsync();
            return Ok(new { rates, timestamp = DateTime.UtcNow });
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
            if (days > 365) days = 365; // Limit to 1 year

            var history = await _exchangeRateService.GetRateHistoryAsync(
                baseCurrency.ToUpper(), targetCurrency.ToUpper(), days);

            return Ok(new { history, days });
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
            var validTimeframes = new[] { "24h", "7d", "30d" };
            
            if (!validTimeframes.Contains(timeframe.ToLower()))
            {
                return BadRequest(new { error = $"Invalid timeframe. Must be one of: {string.Join(", ", validTimeframes)}" });
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
                return StatusCode(500, new { error = "Failed to retrieve top movers", details = ex.Message });
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
    }
}
