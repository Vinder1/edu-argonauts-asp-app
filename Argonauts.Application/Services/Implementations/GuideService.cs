using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Utility.Content;

namespace Argonauts.Application.Services.Implementations;

public class GuideService(DataContainer dataContainer) : IGuideService
{
    private readonly DataContainer _dataContainer = dataContainer;

    public Dictionary<string, string> GetAllGuides()
    {
        return _dataContainer.Guides;
    }

    public string? GetGuide(string key)
    {
        _dataContainer.Guides.TryGetValue(key, out var guide);
        return guide;
    }
}
