using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using ForexRateAlerter.Api;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;

namespace ForexRateAlerter.Tests.Integration
{
    [TestFixture]
    public class AlertControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private string _authToken;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace the database with in-memory database for testing
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _client = _factory.CreateClient();

            // Register a user and get a token for authenticated tests
            await RegisterUserAndLogin();
        }

        private async Task RegisterUserAndLogin()
        {
            var registerDto = new RegisterDto
            {
                Email = "testuser@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            var registerContent = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/auth/register", registerContent);

            var loginDto = new LoginDto
            {
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            var loginContent = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", loginContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
            _authToken = loginResponse.GetProperty("token").GetString();
        }

        [Test]
        public async Task CreateAlert_ValidData_ReturnsCreated()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            var createAlertDto = new CreateAlertDto
            {
                BaseCurrency = "USD",
                TargetCurrency = "MWK",
                Condition = AlertCondition.GreaterThan,
                TargetRate = 1700.0m
            };
            var content = new StringContent(JsonSerializer.Serialize(createAlertDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/alert", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }
    }
}
