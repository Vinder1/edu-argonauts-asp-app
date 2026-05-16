namespace Argonauts.Web.Requests;

/// <summary>
/// Represents a response for user authentication.
/// </summary>
public class LogInResponse
{
    /// <summary>
    /// True if log-in attempt is successful
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Token for authentication.
    /// </summary>
    public string Token { get; init; } = null!;

    /// <summary>
    /// Errors if request is invalid.
    /// </summary>
    public string[] Errors { get; init; } = null!;
}