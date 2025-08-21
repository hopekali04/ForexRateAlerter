using ForexRateAlerter.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class AlertBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AlertBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(1); // Run every hour

        public AlertBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<AlertBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var exchangeRateService = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();
                    var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                    _logger.LogInformation("Starting scheduled forex rate update and alert processing");

                    // Fetch latest exchange rates
                    var ratesFetched = await exchangeRateService.FetchAndStoreLatestRatesAsync();
                    if (ratesFetched)
                    {
                        _logger.LogInformation("Successfully updated exchange rates");

                        // Process alerts
                        var alertsProcessed = await alertService.ProcessAlertsAsync();
                        _logger.LogInformation($"Processed {alertsProcessed} alerts");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to fetch exchange rates");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in alert background service");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}