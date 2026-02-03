using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Infrastructure.Data;
using ForexRateAlerter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace ForexRateAlerter.Tests.Services
{
    [TestFixture]
    public class ExchangeRateServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<ExchangeRateService>> _mockLogger;

        public ExchangeRateServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ForexRateAlerterTestDb_{Guid.NewGuid()}")
                .Options;

            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<ExchangeRateService>>();
        }

        [Test]
        public async Task FetchAndStoreLatestRatesAsync_ShouldFetchAndStoreRates_AndDisplayResults()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var supportedCurrencies = new[] { "USD", "EUR", "GBP", "MWK", "ZAR", "JPY", "CAD", "AUD" };

            // Setup configuration
            _mockConfiguration.Setup(c => c["ExchangeRateApi:ApiKey"]).Returns("test_api_key");
            _mockConfiguration.Setup(c => c["ExchangeRateApi:BaseUrl"]).Returns("https://api.exchangerate-api.com/v4");

            // Mock HTTP responses for each currency - match exact URL pattern
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    // Extract base currency from URL
                    var url = request.RequestUri?.ToString() ?? "";
                    var baseCurrency = supportedCurrencies.FirstOrDefault(c => url.Contains($"latest/{c}"));
                    
                    if (baseCurrency != null)
                    {
                        var mockResponse = CreateMockApiResponse(baseCurrency, supportedCurrencies);
                        var jsonResponse = JsonSerializer.Serialize(mockResponse);
                        
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(jsonResponse)
                        };
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            // Create a fresh database context
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            // Act
            bool result;
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var service = new ExchangeRateService(context, httpClient, _mockConfiguration.Object, _mockLogger.Object);
                result = await service.FetchAndStoreLatestRatesAsync();
            }

            // Assert
            Assert.IsTrue(result, "FetchAndStoreLatestRatesAsync should return true on success");

            // Verify data was stored and display results
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var storedRates = await context.ExchangeRates.ToListAsync();
                
                Assert.IsNotEmpty(storedRates, "Exchange rates should be stored in the database");
                Assert.Greater(storedRates.Count, 0, "At least one rate should be stored");

                // Display results to console
                TestContext.WriteLine("==============================================");
                TestContext.WriteLine("FETCH LATEST EXCHANGE RATES - TEST RESULTS");
                TestContext.WriteLine("==============================================");
                TestContext.WriteLine($"Total Rates Fetched: {storedRates.Count}");
                TestContext.WriteLine($"Fetch Time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                TestContext.WriteLine("----------------------------------------------");

                // Group by base currency for better readability
                var groupedRates = storedRates
                    .GroupBy(r => r.BaseCurrency)
                    .OrderBy(g => g.Key);

                foreach (var group in groupedRates)
                {
                    TestContext.WriteLine($"\nBase Currency: {group.Key}");
                    TestContext.WriteLine("".PadRight(42, '-'));
                    
                    foreach (var rate in group.OrderBy(r => r.TargetCurrency))
                    {
                        TestContext.WriteLine($"  {rate.BaseCurrency}/{rate.TargetCurrency}: {rate.Rate:F4}");
                    }
                }

                TestContext.WriteLine("\n==============================================");
                TestContext.WriteLine("Sample Rate Details:");
                TestContext.WriteLine("----------------------------------------------");
                var sampleRate = storedRates.First();
                TestContext.WriteLine($"  ID: {sampleRate.Id}");
                TestContext.WriteLine($"  Pair: {sampleRate.BaseCurrency}/{sampleRate.TargetCurrency}");
                TestContext.WriteLine($"  Rate: {sampleRate.Rate}");
                TestContext.WriteLine($"  Source: {sampleRate.Source}");
                TestContext.WriteLine($"  Timestamp: {sampleRate.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
                TestContext.WriteLine("==============================================");

                // Verify data integrity
                foreach (var rate in storedRates)
                {
                    Assert.IsNotNull(rate.BaseCurrency, "Base currency should not be null");
                    Assert.IsNotNull(rate.TargetCurrency, "Target currency should not be null");
                    Assert.Greater(rate.Rate, 0, $"Rate for {rate.BaseCurrency}/{rate.TargetCurrency} should be positive");
                    Assert.That(rate.Source, Is.EqualTo("ExchangeRate-API"), "Source should be 'ExchangeRate-API'");
                    Assert.LessOrEqual((DateTime.UtcNow - rate.Timestamp).TotalMinutes, 1, 
                        "Timestamp should be within 1 minute of current time");
                }
            }

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully fetched")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once,
                "Should log success message"
            );
        }

        private object CreateMockApiResponse(string baseCurrency, string[] allCurrencies)
        {
            var rates = new Dictionary<string, decimal>();
            var random = new Random(baseCurrency.GetHashCode()); // Deterministic rates

            foreach (var targetCurrency in allCurrencies)
            {
                // Generate realistic exchange rates
                decimal rate = targetCurrency switch
                {
                    "USD" => baseCurrency == "USD" ? 1.0m : (decimal)(0.8 + random.NextDouble() * 0.4),
                    "EUR" => baseCurrency == "EUR" ? 1.0m : (decimal)(0.85 + random.NextDouble() * 0.3),
                    "GBP" => baseCurrency == "GBP" ? 1.0m : (decimal)(0.7 + random.NextDouble() * 0.4),
                    "MWK" => baseCurrency == "MWK" ? 1.0m : (decimal)(800 + random.NextDouble() * 400),
                    "ZAR" => baseCurrency == "ZAR" ? 1.0m : (decimal)(15 + random.NextDouble() * 5),
                    "JPY" => baseCurrency == "JPY" ? 1.0m : (decimal)(110 + random.NextDouble() * 40),
                    "CAD" => baseCurrency == "CAD" ? 1.0m : (decimal)(1.2 + random.NextDouble() * 0.3),
                    "AUD" => baseCurrency == "AUD" ? 1.0m : (decimal)(1.3 + random.NextDouble() * 0.4),
                    _ => 1.0m
                };

                rates[targetCurrency] = Math.Round(rate, 4);
            }

            return new
            {
                result = "success",
                conversion_rates = rates
            };
        }
    }
}
