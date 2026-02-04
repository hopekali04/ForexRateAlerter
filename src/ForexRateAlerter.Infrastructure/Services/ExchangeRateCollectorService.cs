using ForexRateAlerter.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services;

/// <summary>
/// Background service that collects exchange rates every hour
/// </summary>
public class ExchangeRateCollectorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExchangeRateCollectorService> _logger;
    private readonly TimeSpan _collectionInterval;

    public ExchangeRateCollectorService(
        IServiceProvider serviceProvider,
        ILogger<ExchangeRateCollectorService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var intervalMinutes = configuration.GetValue<int>("ExchangeRateHistory:IntervalMinutes", 60);
        _collectionInterval = TimeSpan.FromMinutes(intervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Exchange Rate Collector Service started. Collection interval: {Interval}", _collectionInterval);

        // Run immediately on startup
        await CollectRatesAsync(stoppingToken);

        // Then run every hour
        using var timer = new PeriodicTimer(_collectionInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CollectRatesAsync(stoppingToken);
        }

        _logger.LogInformation("Exchange Rate Collector Service stopped.");
    }

    private async Task CollectRatesAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Collecting exchange rates at {Time} UTC", DateTime.UtcNow);

            // Create a scope to resolve scoped services
            using var scope = _serviceProvider.CreateScope();
            var historyService = scope.ServiceProvider.GetRequiredService<IExchangeRateHistoryService>();

            await historyService.StoreCurrentRatesAsync();

            _logger.LogInformation("Exchange rate collection completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while collecting exchange rates");
            // Continue running even if one collection fails
        }
    }
}
