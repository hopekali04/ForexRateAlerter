using ForexRateAlerter.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    /// <summary>
    /// Fetches exchange rates from the external API (exchangerate-api.com) every 24 hours.
    /// This complements the ExchangeRateSyncService (hourly synthetic calculations).
    /// Provides a baseline refresh from the actual market data source.
    /// </summary>
    public class ExternalRateFetchService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ExternalRateFetchService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24);

        public ExternalRateFetchService(
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<ExternalRateFetchService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("══════════════════════════════════════════════════");
            _logger.LogInformation("║ External Rate Fetch Service STARTED            ║");
            _logger.LogInformation("║ Cycle Interval: {Interval}                     ║", _period);
            _logger.LogInformation("║ Source: exchangerate-api.com (Market Baseline) ║");
            _logger.LogInformation("══════════════════════════════════════════════════");

            // Initial delay to avoid startup collision with synthetic service
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            using var timer = new PeriodicTimer(_period);

            // Initial run
            await FetchExternalRatesAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await FetchExternalRatesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CRITICAL: External rate fetch cycle failed. Will retry in 24h.");
                }
            }

            _logger.LogInformation("External Rate Fetch Service STOPPED.");
        }

        private async Task FetchExternalRatesAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var exchangeRateService = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();

            try
            {
                _logger.LogInformation("Starting 24h External API Fetch...");
                
                var success = await exchangeRateService.FetchAndStoreLatestRatesAsync();
                
                if (success)
                {
                    _logger.LogInformation("✅ External API Fetch Complete. Fresh market data stored.");
                }
                else
                {
                    _logger.LogWarning("⚠️ External API Fetch returned false. Check API key or network.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External API Fetch Failed");
                throw;
            }
        }
    }
}
