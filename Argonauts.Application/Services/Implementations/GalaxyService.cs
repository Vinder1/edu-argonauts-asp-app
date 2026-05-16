using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="galaxyRepository"></param>
/// <param name="logger"></param>
public class GalaxyService(IGalaxyRepository galaxyRepository, ILogger<GalaxyService> logger) : IGalaxyService
{
    private readonly IGalaxyRepository _galaxyRepository = galaxyRepository
        ?? throw new ArgumentNullException(nameof(galaxyRepository));
    private readonly ILogger<GalaxyService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="GalaxyMissingException"></exception>
    public async Task<int> GetCurrentGalaxyVersionAsync()
    {
        var galaxy = await _galaxyRepository.GetAsync();
        if (galaxy == null)
            return -1;
        return galaxy.Version;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<Galaxy> RegenerateGalaxyAsync()
    {
        var currentGalaxy = await _galaxyRepository.GetAsync();
        var newVersion = (currentGalaxy?.Version + 1) ?? 1;

        var stars = GalaxyGenerator.GenerateStars();
        var newGalaxy = GalaxyGenerator.CreateGalaxy(newVersion, stars);

        await _galaxyRepository.OverrideAsync(newGalaxy);

        _logger.LogWarning("Galaxy is overwritten! New version is {Version}", newVersion);

        return newGalaxy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="GalaxyMissingException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<IEnumerable<Star>> GetAllStarsAsync()
    {
        var galaxy = await _galaxyRepository.GetAsync();
        return galaxy?.Stars.GetAllStars() ?? [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angleMilliradians"></param>
    /// <returns></returns>
    /// <exception cref="GalaxyMissingException"></exception>
    public async Task<Star?> FindStarAsync(int radius, int angleMilliradians)
    {
        var galaxy = await galaxyRepository.GetAsync();
        return galaxy?.Stars.Find(radius, angleMilliradians);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public async Task<bool> StarExistsAsync(int radius, int angle)
    {
        return (await FindStarAsync(radius, angle)) != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    /// <exception cref="GalaxyMissingException"></exception>
    public async Task<IEnumerable<Star>> GetStarsByRadiusRangeAsync(int minRadius, int maxRadius)
    {
        var galaxy = await _galaxyRepository.GetAsync();
        return galaxy?.Stars.GetStarsByRadiusRange(minRadius, maxRadius) ?? [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    /// <exception cref="GalaxyMissingException"></exception>
    public async Task<IEnumerable<Star>> GetStarsNearStarAsync(Star center, int radius)
    {
        var galaxy = await galaxyRepository.GetAsync();
        return galaxy?.Stars.FindNearbyStars(center.Radius, center.AngleMilliradians, radius) ?? [];
    }
}