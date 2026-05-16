using Argonauts.Application.Dto;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="spaceshipRepository"></param>
/// <param name="playerService"></param>
/// <param name="galaxyService"></param>
/// <param name="logger"></param>
public class SpaceshipService(
    ISpaceshipRepository spaceshipRepository,
    IPlayerService playerService,
    IGalaxyService galaxyService,
    ILogger<SpaceshipService> logger) : ISpaceshipService
{
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));
    private readonly IPlayerService _playerService = playerService
        ?? throw new ArgumentNullException(nameof(playerService));
    private readonly IGalaxyService _galaxyService = galaxyService
        ?? throw new ArgumentNullException(nameof(galaxyService));
    private readonly ILogger<SpaceshipService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ServiceActionResult> CreateSpaceshipAsync(Guid ownerId)
    {
        if (!await _playerService.PlayerExistsAsync(ownerId))
        {
            return ServiceActionResult.Invalid($"Cannot create spaceship: Player {ownerId} does not exist");
        }

        var currentGalaxyVersion = await _galaxyService.GetCurrentGalaxyVersionAsync();

        if (await HasActiveSpaceshipAsync(ownerId, currentGalaxyVersion))
        {
            return ServiceActionResult.Invalid($"Cannot create spaceship: Player {ownerId} already has one");
        }

        await _spaceshipRepository.CreateAsync(ownerId, new Spaceship
        {
            GalaxyVersion = currentGalaxyVersion,
            OwnerId = ownerId
        });

        _logger.LogInformation("Spaceship (owner={UserId}): Created for Galaxy {GalaxyVersion}", ownerId, currentGalaxyVersion);

        return ServiceActionResult.Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public Task<Spaceship?> GetSpaceshipAsync(Guid ownerId)
    {
        return _spaceshipRepository.GetAsync(ownerId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Spaceship>> GetAllSpaceshipsAsync()
    {
        return _spaceshipRepository.GetAllAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IEnumerable<Spaceship>> GetAllOnStarAsync(Star star)
    {
        if (!await _galaxyService.StarExistsAsync(star))
        {
            throw new ArgumentException("Star not found");
        }

        var spaceships = await _spaceshipRepository.GetAllOnStarAsync(star.Radius, star.AngleMilliradians);
        return spaceships;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<int> CleanupInactiveSpaceshipsAsync()
    {
        var currentGalaxyVersion = await _galaxyService.GetCurrentGalaxyVersionAsync();

        var beforeCleanup = await _spaceshipRepository.GetAllAsync();
        var beforeCount = beforeCleanup.Count();

        await _spaceshipRepository.DeleteInactiveAsync(currentGalaxyVersion);

        var afterCleanup = await _spaceshipRepository.GetAllAsync();
        var afterCount = afterCleanup.Count();

        var removedCount = beforeCount - afterCount;

        return removedCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="currentGalaxyVersion"></param>
    /// <returns></returns>
    public async Task<bool> HasActiveSpaceshipAsync(Guid ownerId, int currentGalaxyVersion)
    {
        try
        {
            var ship = await _spaceshipRepository.GetAsync(ownerId);
            if (ship == null)
                return false;
            return ship.GalaxyVersion >= currentGalaxyVersion;
        }
        catch
        {
            return false;
        }
    }
}