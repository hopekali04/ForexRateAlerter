using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ForexRateAlerter.Core.DTOs;
using ForexRateAlerter.Core.Interfaces;
using ForexRateAlerter.Core.Models;
using ForexRateAlerter.Infrastructure.Data;
using ForexRateAlerter.Infrastructure.Services;

namespace ForexRateAlerter.Tests.Services
{
    [TestFixture]
    public class AlertServiceTests
    {
        private ApplicationDbContext _context;
        private Mock<IExchangeRateService> _mockExchangeRateService;
        private Mock<IEmailService> _mockEmailService;
        private Mock<ILogger<AlertService>> _mockLogger;
        private AlertService _alertService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockExchangeRateService = new Mock<IExchangeRateService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<AlertService>>();

            _alertService = new AlertService(_context, _mockExchangeRateService.Object, 
                _mockEmailService.Object, _mockLogger.Object);

            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashedpassword"
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        [Test]
        public async Task CreateAlertAsync_ValidData_ReturnsAlertResponseDto()
        {
            // Arrange
            var createAlertDto = new CreateAlertDto
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Condition = AlertCondition.LessThan,
                TargetRate = 0.85m
            };

            // Act
            var result = await _alertService.CreateAlertAsync(1, createAlertDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.BaseCurrency, Is.EqualTo("USD"));
            Assert.That(result.TargetCurrency, Is.EqualTo("EUR"));
            Assert.That(result.TargetRate, Is.EqualTo(0.85m));
            Assert.That(result.IsActive, Is.True);
        }

        [Test]
        public async Task ProcessAlertsAsync_TriggeredAlert_SendsEmailAndLogsAlert()
        {
            // Arrange
            var alert = new Alert
            {
                UserId = 1,
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Condition = AlertCondition.LessThan,
                TargetRate = 0.90m,
                IsActive = true
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();

            var exchangeRate = new ExchangeRate
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Rate = 0.85m,
                Timestamp = DateTime.UtcNow
            };

            _mockExchangeRateService.Setup(x => x.GetLatestRateAsync("USD", "EUR"))
                .ReturnsAsync(exchangeRate);
            _mockEmailService.Setup(x => x.SendAlertEmailAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var processedCount = await _alertService.ProcessAlertsAsync();

            // Assert
            Assert.That(processedCount, Is.EqualTo(1));
            _mockEmailService.Verify(x => x.SendAlertEmailAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Once);

            var alertLog = await _context.AlertLogs.FirstOrDefaultAsync();
            Assert.That(alertLog, Is.Not.Null);
            Assert.That(alertLog.EmailSent, Is.True);
        }

        [Test]
        public async Task GetUserAlertsAsync_ValidUserId_ReturnsUserAlerts()
        {
            // Arrange
            var alerts = new[]
            {
                new Alert { UserId = 1, BaseCurrency = "USD", TargetCurrency = "EUR", Condition = AlertCondition.LessThan, TargetRate = 0.85m },
                new Alert { UserId = 1, BaseCurrency = "GBP", TargetCurrency = "USD", Condition = AlertCondition.GreaterThan, TargetRate = 1.30m },
                new Alert { UserId = 2, BaseCurrency = "USD", TargetCurrency = "JPY", Condition = AlertCondition.EqualTo, TargetRate = 110m }
            };

            _context.Alerts.AddRange(alerts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _alertService.GetUserAlertsAsync(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(a => a.BaseCurrency == "USD" || a.BaseCurrency == "GBP"), Is.True);
        }
    }
}
