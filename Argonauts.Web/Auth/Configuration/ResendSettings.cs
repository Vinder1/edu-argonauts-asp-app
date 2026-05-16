namespace Argonauts.Web.Auth.Configuration;

/// <summary>
/// Configuration for Resend email service.
/// </summary>
public class ResendSettings
{
    /// <summary>
    /// Resend API key.
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// The sender email address (from).
    /// </summary>
    public string FromEmail { get; set; } = null!;

    /// <summary>
    /// The sender display name.
    /// </summary>
    public string FromName { get; set; } = null!;
}
