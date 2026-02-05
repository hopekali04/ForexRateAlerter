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
            _logger.LogInformation("Alert Monitoring Service Started. Check Interval: {Interval}", _period);

            // Wait for initial startup to complete
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

                    _logger.LogInformation("Starting scheduled alert check");

                    // Process alerts (rates are now managed by ExchangeRateSyncService)
                    var alertsProcessed = await alertService.ProcessAlertsAsync();
                    _logger.LogInformation("Processed {AlertCount} alerts", alertsProcessed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in alert monitoring service");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}