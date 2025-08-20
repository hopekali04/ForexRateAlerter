namespace ForexRateAlerter.Core.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAlertEmailAsync(string toEmail, string baseCurrency, string targetCurrency, 
            decimal currentRate, decimal targetRate, string condition);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string firstName);
    }
}
