namespace Argonauts.Web.Auth.Abstractions;

/// <summary>
/// Service for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a welcome email to a newly registered player.
    /// </summary>
    Task SendWelcomeEmailAsync(string email, string name);
}
