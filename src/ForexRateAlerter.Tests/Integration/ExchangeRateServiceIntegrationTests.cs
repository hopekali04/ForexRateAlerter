using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ForexRateAlerter.Infrastructure.Data;
using ForexRateAlerter.Infrastructure.Services;

namespace ForexRateAlerter.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class ExchangeRateServiceIntegrationTests
    {
        private ApplicationDbContext _context;
        private IConfiguration _configuration;
        private ILogger<ExchangeRateService> _logger;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            // Build configuration from appsettings and environment variables
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();
            
            _configuration = configBuilder.Build();

            // Create in-memory database for integration test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"IntegrationTestDb_{Guid.NewGuid()}")
                .Options;
            
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Setup logger
            var loggerFactory = LoggerFactory.Create(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            _logger = loggerFactory.CreateLogger<ExchangeRateService>();

            // Real HTTP client - no mocking
            _httpClient = new HttpClient();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
            _httpClient?.Dispose();
        }

        [Test]
        [Category("Live API")]
        public async Task FetchAndStoreLatestRatesAsync_RealAPI_ShouldFetchAndStoreRates()
        {
            // Arrange
            var apiKey = _configuration["ExchangeRateApi:ApiKey"];
            var baseUrl = _configuration["ExchangeRateApi:BaseUrl"];

            // Verify configuration is available
            Assert.IsNotNull(apiKey, "ExchangeRateApi:ApiKey must be set in configuration or environment variables");
            Assert.IsNotNull(baseUrl, "ExchangeRateApi:BaseUrl must be set in configuration");
            Assert.IsNotEmpty(apiKey.Trim(), "API Key cannot be empty");

            TestContext.WriteLine("==============================================");
            TestContext.WriteLine("LIVE API INTEGRATION TEST - EXCHANGE RATES");
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine($"API Base URL: {baseUrl}");
            TestContext.WriteLine($"API Key: {apiKey.Substring(0, Math.Min(10, apiKey.Length))}...");
            TestContext.WriteLine($"Test Started: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            TestContext.WriteLine("----------------------------------------------\n");

            var service = new ExchangeRateService(_context, _httpClient, _configuration, _logger);

            // Act
            TestContext.WriteLine("Fetching exchange rates from live API...\n");
            var startTime = DateTime.UtcNow;
            var result = await service.FetchAndStoreLatestRatesAsync();
            var duration = (DateTime.UtcNow - startTime).TotalSeconds;

            // Assert
            Assert.IsTrue(result, "FetchAndStoreLatestRatesAsync should return true on success");
            TestContext.WriteLine($"✓ API call successful (took {duration:F2} seconds)\n");

            // Retrieve and verify stored rates - query the same context used by the service
            var storedRates = await _context.ExchangeRates.ToListAsync();
            
            Assert.IsNotEmpty(storedRates, "Exchange rates should be stored in the database");
            TestContext.WriteLine($"✓ Stored {storedRates.Count} exchange rates in database\n");

            // Display comprehensive results
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine("FETCHED EXCHANGE RATES - LIVE DATA");
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine($"Total Rates Fetched: {storedRates.Count}");
            TestContext.WriteLine($"Fetch Completed: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            TestContext.WriteLine($"Duration: {duration:F2} seconds");
            TestContext.WriteLine("----------------------------------------------");

            // Group by base currency for organized display
            var groupedRates = storedRates
                .GroupBy(r => r.BaseCurrency)
                .OrderBy(g => g.Key)
                .ToList();

            TestContext.WriteLine($"\nBase Currencies: {string.Join(", ", groupedRates.Select(g => g.Key))}");
            TestContext.WriteLine($"Total Currency Pairs: {storedRates.Count}");
            TestContext.WriteLine("\n----------------------------------------------");

            foreach (var group in groupedRates)
            {
                TestContext.WriteLine($"\n📊 Base Currency: {group.Key}");
                TestContext.WriteLine("".PadRight(42, '-'));
                
                var rates = group.OrderBy(r => r.TargetCurrency).ToList();
                foreach (var rate in rates)
                {
                    TestContext.WriteLine($"  {rate.BaseCurrency}/{rate.TargetCurrency}: {rate.Rate:F4}");
                }
                
                TestContext.WriteLine($"  Total pairs: {rates.Count}");
            }

            // Display sample rate with full details
            TestContext.WriteLine("\n==============================================");
            TestContext.WriteLine("SAMPLE RATE DETAILS (Full Record)");
            TestContext.WriteLine("==============================================");
            var sampleRate = storedRates.OrderBy(r => r.BaseCurrency).ThenBy(r => r.TargetCurrency).First();
            TestContext.WriteLine($"  ID: {sampleRate.Id}");
            TestContext.WriteLine($"  Currency Pair: {sampleRate.BaseCurrency}/{sampleRate.TargetCurrency}");
            TestContext.WriteLine($"  Exchange Rate: {sampleRate.Rate}");
            TestContext.WriteLine($"  Data Source: {sampleRate.Source}");
            TestContext.WriteLine($"  Timestamp (UTC): {sampleRate.Timestamp:yyyy-MM-dd HH:mm:ss}");
            TestContext.WriteLine($"  Age: {(DateTime.UtcNow - sampleRate.Timestamp).TotalSeconds:F1} seconds");

            // Data quality validation
            TestContext.WriteLine("\n==============================================");
            TestContext.WriteLine("DATA QUALITY VALIDATION");
            TestContext.WriteLine("==============================================");
            
            var validationResults = new Dictionary<string, bool>();
            
            // Check 1: All rates are positive
            var allPositive = storedRates.All(r => r.Rate > 0);
            validationResults["All rates positive"] = allPositive;
            TestContext.WriteLine($"  ✓ All rates positive: {allPositive}");
            
            // Check 2: All have valid currency codes
            var allHaveCurrencies = storedRates.All(r => 
                !string.IsNullOrWhiteSpace(r.BaseCurrency) && 
                !string.IsNullOrWhiteSpace(r.TargetCurrency));
            validationResults["Valid currency codes"] = allHaveCurrencies;
            TestContext.WriteLine($"  ✓ Valid currency codes: {allHaveCurrencies}");
            
            // Check 3: All have recent timestamps (within last 5 minutes)
            var allRecent = storedRates.All(r => (DateTime.UtcNow - r.Timestamp).TotalMinutes < 5);
            validationResults["Recent timestamps"] = allRecent;
            TestContext.WriteLine($"  ✓ Recent timestamps: {allRecent}");
            
            // Check 4: All have correct source
            var correctSource = storedRates.All(r => r.Source == "ExchangeRate-API");
            validationResults["Correct source attribution"] = correctSource;
            TestContext.WriteLine($"  ✓ Correct source: {correctSource}");
            
            // Check 5: Reasonable rate ranges
            var reasonableRates = storedRates.All(r => r.Rate >= 0.0001m && r.Rate <= 10000m);
            validationResults["Reasonable rate ranges"] = reasonableRates;
            TestContext.WriteLine($"  ✓ Reasonable rates: {reasonableRates}");

            // Check 6: No duplicate pairs at same timestamp
            var noDuplicates = storedRates
                .GroupBy(r => new { r.BaseCurrency, r.TargetCurrency, r.Timestamp })
                .All(g => g.Count() == 1);
            validationResults["No duplicates"] = noDuplicates;
            TestContext.WriteLine($"  ✓ No duplicates: {noDuplicates}");

            TestContext.WriteLine("\n==============================================");
            var allPassed = validationResults.All(kvp => kvp.Value);
            TestContext.WriteLine($"Overall Quality: {(allPassed ? "✓ PASSED" : "✗ FAILED")}");
            TestContext.WriteLine("==============================================\n");

            // Statistical summary
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine("STATISTICAL SUMMARY");
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine($"  Min Rate: {storedRates.Min(r => r.Rate):F4}");
            TestContext.WriteLine($"  Max Rate: {storedRates.Max(r => r.Rate):F4}");
            TestContext.WriteLine($"  Average Rate: {storedRates.Average(r => r.Rate):F4}");
            TestContext.WriteLine($"  Median Rate: {CalculateMedian(storedRates.Select(r => r.Rate).ToList()):F4}");
            TestContext.WriteLine("==============================================\n");

            // Assert all validations passed
            Assert.IsTrue(allPositive, "All rates should be positive");
            Assert.IsTrue(allHaveCurrencies, "All rates should have valid currency codes");
            Assert.IsTrue(allRecent, "All rates should have recent timestamps");
            Assert.IsTrue(correctSource, "All rates should have correct source attribution");
            Assert.IsTrue(reasonableRates, "All rates should be within reasonable ranges");
            Assert.IsTrue(noDuplicates, "There should be no duplicate rate entries");
        }

        private decimal CalculateMedian(List<decimal> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            int count = sorted.Count;
            if (count == 0) return 0;
            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;
            }
            return sorted[count / 2];
        }
    }
}
