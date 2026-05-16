namespace Argonauts.Application.Services.Abstractions;

public interface IGuideService
{
    Dictionary<string, string> GetAllGuides();
    string? GetGuide(string key);
}
