using Destructurama.Attributed;

namespace Argonauts.Web.Requests;

/// <summary>
/// Represents a request for user authentication with login credentials.
/// </summary>
public class LogInRequest
{
    /// <summary>
    /// Login for authentication.
    /// </summary>
    public string Login { get; init; } = null!;

    /// <summary>
    /// Password for authentication.
    /// </summary>
    [LogMasked(ShowFirst = 3, ShowLast = 2)]
    public string Password { get; init; } = null!;
}