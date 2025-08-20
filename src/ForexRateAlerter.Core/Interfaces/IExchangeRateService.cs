using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Core.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeRate?> GetLatestRateAsync(string baseCurrency, string targetCurrency);
        Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync();
        Task<bool> FetchAndStoreLatestRatesAsync();
        Task<IEnumerable<ExchangeRate>> GetRateHistoryAsync(string baseCurrency, string targetCurrency, int days = 30);
    }
}
