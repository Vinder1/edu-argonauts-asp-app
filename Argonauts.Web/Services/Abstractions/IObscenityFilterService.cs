namespace Argonauts.Web.Services.Abstractions;

/// <summary>
/// Filters obscene words from text using the ObscenitiesFilterService.
/// </summary>
public interface IObscenityFilterService
{
    /// <summary>
    /// Filters obscene words from the given text.
    /// </summary>
    Task<string> FilterAsync(string text, CancellationToken cancellationToken = default);
}
