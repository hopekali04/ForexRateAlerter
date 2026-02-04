using ForexRateAlerter.Core.DTOs;

namespace ForexRateAlerter.Core.Interfaces;

public interface IExchangeRateHistoryService
{
    /// <summary>
    /// Fetches current rates from external API and stores them in history table
    /// Called by background job every hour
    /// </summary>
    Task StoreCurrentRatesAsync();
    
    /// <summary>
    /// Gets top currency movers for a specific timeframe
    /// </summary>
    /// <param name="timeframe">24h, 7d, or 30d</param>
    /// <param name="limit">Number of top movers to return (default 5)</param>
    Task<IEnumerable<TopMoverDto>> GetTopMoversAsync(string timeframe, int limit = 5);
    
    /// <summary>
    /// Gets historical rates for a currency pair from the history table
    /// </summary>
    /// <param name="baseCurrency">Base currency code</param>
    /// <param name="targetCurrency">Target currency code</param>
    /// <param name="days">Number of days to look back (default 30)</param>
    Task<IEnumerable<ExchangeRateDto>> GetHistoricalRatesAsync(string baseCurrency, string targetCurrency, int days = 30);
}
