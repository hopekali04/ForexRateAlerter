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
}
