using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ForexRateAlerter.Infrastructure.Services;

namespace ForexRateAlerter.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class EmailServiceIntegrationTests
    {
        private IConfiguration _configuration;
        private ILogger<EmailService> _logger;

        [SetUp]
        public void SetUp()
        {
            // Build configuration from appsettings and environment variables
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();
            
            _configuration = configBuilder.Build();

            // Setup logger
            var loggerFactory = LoggerFactory.Create(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            _logger = loggerFactory.CreateLogger<EmailService>();
        }

        [Test]
        [Category("Live SMTP")]
        public async Task SendTestEmail_Should_Succeed()
        {
            // Arrange
            var emailService = new EmailService(_configuration, _logger);
            var recipientEmail = _configuration["Email:SmtpUser"];

            Assert.IsNotNull(recipientEmail, "Email:SmtpUser must be set in configuration.");

            TestContext.WriteLine("==============================================");
            TestContext.WriteLine("LIVE SMTP INTEGRATION TEST");
            TestContext.WriteLine("==============================================");
            TestContext.WriteLine($"Sending test email to: {recipientEmail}");
            TestContext.WriteLine("----------------------------------------------\n");

            // Act
            var result = await emailService.SendTestEmailAsync(recipientEmail);

            // Assert
            Assert.IsTrue(result, "The test email should be sent successfully.");
            
            TestContext.WriteLine("✓ SMTP Test Email Sent Successfully");
            TestContext.WriteLine("\n==============================================");
        }
    }
}
