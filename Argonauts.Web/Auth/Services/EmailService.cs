using Argonauts.Web.Auth.Abstractions;
using Argonauts.Web.Auth.Configuration;
using Microsoft.Extensions.Options;
using Resend;

namespace Argonauts.Web.Auth.Services;

/// <summary>
/// Sends emails using the Resend API.
/// </summary>
/// <param name="resend"></param>
/// <param name="settings"></param>
/// <param name="logger"></param>
public class EmailService(IResend resend, IOptions<ResendSettings> settings, ILogger<EmailService> logger) : IEmailService
{
    private readonly IResend _resend = resend 
        ?? throw new ArgumentNullException(nameof(resend));
    private readonly ResendSettings _settings = settings.Value 
        ?? throw new ArgumentNullException(nameof(settings));
    private readonly ILogger<EmailService> _logger = logger 
        ?? throw new ArgumentNullException(nameof(logger));


    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="name"></param>
    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        var message = new EmailMessage
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>"
        };
        message.To.Add(email);
        message.Subject = "Добро пожаловать в 'Аргонавты'!";
        message.HtmlBody = $"""
            <h1>Welcome, {name}!</h1>
            <p>Привет! Добро пожаловать в 'Аргонавты'!</p>
            <p>Твоя почта была использована для регистрации, хочешь ли ты этого или нет.</p>
            <p>Увидимся среди звёзд!</p>
            <p>- единственный и неповторимый разработчик этого шедевра</p>
            """;

        await _resend.EmailSendAsync(message);
        _logger.LogInformation("Welcome email sent to {Email}", email);
    }
}
