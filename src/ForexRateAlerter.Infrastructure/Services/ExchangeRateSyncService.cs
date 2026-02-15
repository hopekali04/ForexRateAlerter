using System.Net.Http;
using System.Text.Json;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForexRateAlerter.Infrastructure.Services
{
    /// <summary>
    /// Fetches base USD rates and calculates synthetic cross-rates for all supported currency pairs.
    /// Acts as the central pricing engine for the application using triangular arbitrage.
    /// </summary>
    public class ExchangeRateSyncService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ExternalApiSettings _apiSettings;
        private readonly ILogger<ExchangeRateSyncService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        // FinTech Standard: Hourly calculation cycle as per 'resolution=1h' requirement
        private readonly TimeSpan _period = TimeSpan.FromHours(1);

        public ExchangeRateSyncService(
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<ExternalApiSettings> apiSettings,
            ILogger<ExchangeRateSyncService> logger
        )
        {
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _apiSettings = apiSettings.Value;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("═══════════════════════════════════════════════════════");
            _logger.LogInformation("║ Synthetic Exchange Rate Engine STARTED              ║");
            _logger.LogInformation(
                "║ Cycle Interval: {Interval}                          ║",
                _period
            );
            _logger.LogInformation("║ Algorithm: Triangular Arbitrage (USD Base)          ║");
            _logger.LogInformation("═══════════════════════════════════════════════════════");

            try
            {
                // Initial run (immediate)
                _logger.LogInformation("Starting INITIAL calculation cycle...");
                await SyncAndCalculateRatesAsync(stoppingToken);
                _logger.LogInformation("Initial cycle COMPLETE. Next cycle in {Delay}", _period);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FATAL ERROR during initial sync. Service cannot continue.");
                throw; // Rethrow to surface configuration issues
            }

            using var timer = new PeriodicTimer(_period);

            while (
                await timer.WaitForNextTickAsync(stoppingToken)
                && !stoppingToken.IsCancellationRequested
            )
            {
                try
                {
                    await SyncAndCalculateRatesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "CRITICAL: Synthetic engine cycle failed. Will retry next cycle."
                    );
                }
            }

            _logger.LogInformation("Synthetic Exchange Rate Engine STOPPED.");
        }

        private async Task SyncAndCalculateRatesAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient("FxRatesApi");

            // 1. Fetch RAW USD Data (The "Truth" Source)
            // We force USD base because it's the standard interbank peg.
            const string masterBase = "USD";
            var currencies = string.Join(",", _apiSettings.SupportedCurrencies);

            try
            {
                _logger.LogInformation("Fetching master pricing feed for {Base}...", masterBase);

                // Construct URL based on docs: /latest?base=USD&currencies=...&resolution=1h&places=6
                // Note: 'amount' defaults to 1, 'format' defaults to json
                var url =
                    $"latest?base={masterBase}&currencies={currencies}&resolution=1h&places=6&api_key={_apiSettings.Key}";

                var response = await client.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                var data = JsonSerializer.Deserialize<FxRatesApiResponse>(content, _jsonOptions);

                if (data?.Success != true || data.Rates == null)
                {
                    _logger.LogWarning(
                        "Master feed empty or invalid. Success: {Success}",
                        data?.Success
                    );
                    return;
                }

                var timestamp = DateTime.UtcNow;
                int updates = 0;
                int historyInserts = 0;
                bool heartbeatUpdates = false;

                // 2. Load Current State (For Change Detection)
                // Use database-side grouping to get only the latest rate for each pair
                var existingRates = await context
                    .ExchangeRates.GroupBy(r => new { r.BaseCurrency, r.TargetCurrency })
                    .Select(g => g.OrderByDescending(r => r.Timestamp).First())
                    .ToDictionaryAsync(
                        r => $"{r.BaseCurrency}-{r.TargetCurrency}",
                        r => r,
                        stoppingToken
                    );

                // 3. Matrix Calculation: O(N^2)
                // We generate a rate for EVERY permutation of our supported currencies
                foreach (var baseSym in _apiSettings.SupportedCurrencies)
                {
                    foreach (var targetSym in _apiSettings.SupportedCurrencies)
                    {
                        if (baseSym == targetSym)
                            continue;

                        // SAFETY: Check if we have the ingredients
                        // logic: Rate(A->B) = Rate(USD->B) / Rate(USD->A)
                        // Handle USD explicitly since it won't be in the rates dictionary when USD is the base
                        decimal usdToBaseRate;
                        if (baseSym == masterBase)
                            usdToBaseRate = 1m;
                        else if (!data.Rates.TryGetValue(baseSym, out usdToBaseRate))
                            continue;

                        decimal usdToTargetRate;
                        if (targetSym == masterBase)
                            usdToTargetRate = 1m;
                        else if (!data.Rates.TryGetValue(targetSym, out usdToTargetRate))
                            continue;

                        if (usdToBaseRate == 0)
                            continue; // Divide by zero guard

                        // FINTECH MANDATE: High precision intermediate calculation
                        // We round to 6 decimals as requested in the API call "places=6" to match input precision
                        decimal calculatedRate = Math.Round(usdToTargetRate / usdToBaseRate, 6);

                        string pairKey = $"{baseSym}-{targetSym}";
                        string sourceLabel = "FxRates-Synthetic";

                        // 4. Persistence Logic
                        if (existingRates.TryGetValue(pairKey, out var existingEntity))
                        {
                            // Change Detection: Only log history if price moved
                            // We use a small epsilon for floating point comparison safe-guarding
                            if (Math.Abs(existingEntity.Rate - calculatedRate) > 0.000001m)
                            {
                                // A. Add to History (Preserve OLD state in history before updating)
                                // Actually, standard practice means we log the NEW state to history as a point-in-time snapshot
                                context.ExchangeRateHistory.Add(
                                    new Core.Models.ExchangeRateHistory
                                    {
                                        BaseCurrency = baseSym,
                                        TargetCurrency = targetSym,
                                        Rate = calculatedRate,
                                        Source = sourceLabel,
                                        CreatedAt = timestamp,
                                    }
                                );
                                historyInserts++;

                                // B. Update Latest Reference
                                existingEntity.Rate = calculatedRate;
                                existingEntity.Timestamp = timestamp;
                                existingEntity.Source = sourceLabel;
                                updates++;
                            }
                            else
                            {
                                // Heartbeat update
                                existingEntity.Timestamp = timestamp;
                                heartbeatUpdates = true;
                            }
                        }
                        else
                        {
                            // New Pair Found (First Run)
                            var newRate = new Core.Models.ExchangeRate
                            {
                                BaseCurrency = baseSym,
                                TargetCurrency = targetSym,
                                Rate = calculatedRate,
                                Timestamp = timestamp,
                                Source = sourceLabel,
                            };
                            context.ExchangeRates.Add(newRate);

                            // Initial history point
                            context.ExchangeRateHistory.Add(
                                new Core.Models.ExchangeRateHistory
                                {
                                    BaseCurrency = baseSym,
                                    TargetCurrency = targetSym,
                                    Rate = calculatedRate,
                                    Source = sourceLabel,
                                    CreatedAt = timestamp,
                                }
                            );
                            updates++;
                        }
                    }
                }

                if (updates > 0 || historyInserts > 0 || heartbeatUpdates)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(
                        "Synthetic Cycle Complete. Updated {UpdateCount} rates. Generated {HistoryCount} history points.",
                        updates,
                        historyInserts
                    );
                }
                else
                {
                    _logger.LogInformation("Synthetic Cycle Complete. No rate changes detected.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed Failure in Pricing Engine.");
            }
        }
    }

    public class FxRatesApiResponse
    {
        public bool Success { get; set; }
        public long Timestamp { get; set; }
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
