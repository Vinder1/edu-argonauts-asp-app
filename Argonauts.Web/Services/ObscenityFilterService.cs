using Argonauts.Web.Services.Abstractions;

namespace Argonauts.Web.Services;

/// <summary>
/// Filters obscene words by calling the ObscenitiesFilterService HTTP API.
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class ObscenityFilterService(
    HttpClient httpClient,
    ILogger<ObscenityFilterService> logger
) : IObscenityFilterService
{
    private readonly HttpClient _httpClient = httpClient 
        ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<ObscenityFilterService> _logger = logger 
        ?? throw new ArgumentNullException(nameof(logger));


    /// <summary>
    /// 
    /// </summary>
    public async Task<string> FilterAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/", new { text }, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return result ?? text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ObscenitiesFilterService call failed, returning original text");
            return text;
        }
    }
}
