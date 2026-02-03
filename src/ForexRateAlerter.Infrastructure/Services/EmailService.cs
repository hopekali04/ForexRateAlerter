using System.Net;
using System.Net.Mail;
using ForexRateAlerter.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ForexRateAlerter.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAlertEmailAsync(string toEmail, string baseCurrency, string targetCurrency, 
            decimal currentRate, decimal targetRate, string condition)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var subject = $"Forex Alert Triggered: {baseCurrency}/{targetCurrency}";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #2563eb;'>Forex Rate Alert Triggered!</h2>
                        <div style='background: #f8fafc; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <p><strong>Currency Pair:</strong> {baseCurrency}/{targetCurrency}</p>
                            <p><strong>Current Rate:</strong> {currentRate:F4}</p>
                            <p><strong>Target Rate:</strong> {targetRate:F4}</p>
                            <p><strong>Condition:</strong> {condition}</p>
                            <p><strong>Triggered At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                        </div>
                        <p>Your alert condition has been met. Consider taking action on your forex position.</p>
                        <p style='color: #64748b; font-size: 12px;'>
                            This is an automated message from Forex Rate Alerter.
                        </p>
                    </body>
                    </html>";

                var message = new MailMessage(fromEmail!, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send alert email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string firstName)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var subject = "Welcome to Forex Rate Alerter!";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #2563eb;'>Welcome to Forex Rate Alerter, {firstName}!</h2>
                        <p>Thank you for registering with our service. You can now:</p>
                        <ul>
                            <li>Set up custom forex rate alerts</li>
                            <li>Monitor real-time exchange rates</li>
                            <li>Receive instant notifications when your target rates are hit</li>
                        </ul>
                        <p>Get started by creating your first alert!</p>
                        <p style='color: #64748b; font-size: 12px;'>
                            This is an automated message from Forex Rate Alerter.
                        </p>
                    </body>
                    </html>";

                var message = new MailMessage(fromEmail!, toEmail, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(message);
                return true;
            }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send welcome email to {toEmail}");
                            return false;
                        }
                    }
            
                    public async Task<bool> SendTestEmailAsync(string toEmail)
                    {
                        try
                        {
                            var smtpHost = _configuration["Email:SmtpHost"];
                            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                            var smtpUser = _configuration["Email:SmtpUser"];
                            var smtpPass = _configuration["Email:SmtpPassword"];
                            var fromEmail = _configuration["Email:FromEmail"];
            
                            using var client = new SmtpClient(smtpHost, smtpPort)
                            {
                                Credentials = new NetworkCredential(smtpUser, smtpPass),
                                EnableSsl = true
                            };
            
                            var subject = "SMTP Connection Test";
                            var body = "This is a test email to verify the SMTP connection.";
            
                            var message = new MailMessage(fromEmail!, toEmail, subject, body);
            
                            await client.SendMailAsync(message);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to send test email to {toEmail}");
                            return false;
                        }
                    }
                }
            }
            