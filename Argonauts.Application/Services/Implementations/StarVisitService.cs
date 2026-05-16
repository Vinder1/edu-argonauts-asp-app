using Argonauts.Application.Dto;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="starVisitRepository"></param>
/// <param name="spaceshipRepository"></param>
/// <param name="galaxyService"></param>
/// <param name="logger"></param>
public class StarVisitService(
    IStarVisitRepository starVisitRepository,
    ISpaceshipRepository spaceshipRepository,
    IGalaxyService galaxyService,
    ILogger<StarVisitService> logger) : IStarVisitService
{
    private readonly IStarVisitRepository _starVisitRepository = starVisitRepository
        ?? throw new ArgumentNullException(nameof(starVisitRepository));
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));
    private readonly IGalaxyService _galaxyService = galaxyService
        ?? throw new ArgumentNullException(nameof(galaxyService));
    private readonly ILogger<StarVisitService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visit"></param>
    /// <returns></returns>
    public Task Create(SpaceshipStarVisit visit)
    {
        return _starVisitRepository.Create(visit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    /// <exception cref="StarNotFoundException"></exception>
    public async Task<ServiceActionResult> Create(Guid ownerId)
    {
        var spaceship = await _spaceshipRepository.GetAsync(ownerId);
        if (spaceship == null)
            return ServiceActionResult.Invalid("Spaceship not found");

        var star = new Star
        {
            Radius = spaceship.LocatedRadius,
            AngleMilliradians = spaceship.LocatedAngleMilliradians
        };
        
        if (!await _galaxyService.StarExistsAsync(star))
        {
            return ServiceActionResult.Invalid("Star, where the spaceship is located, not found");
        }

        var active = await GetActiveFor(ownerId, star);
        if (active != null)
        {
            return ServiceActionResult.Invalid($"You have already been here within 24 hours. {active.Value.VisitedAt.AddDays(1) - DateTime.Now} left");
        }

        var spaceshipVisit = new SpaceshipStarVisit
        {
            GalaxyVersion = await _galaxyService.GetCurrentGalaxyVersionAsync(),
            Spaceship = new Spaceship { OwnerId = ownerId },
            Star = star
        };

        _logger.LogInformation("Spaceship (Owner={UserId}): Visited {@Star}", ownerId, star);

        await Create(spaceshipVisit);
        return ServiceActionResult.Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="star"></param>
    /// <returns></returns>
    /// <exception cref="StarNotFoundException"></exception>
    public async Task<SpaceshipStarVisit?> GetActiveFor(Guid ownerId, Star star)
    {
        if (!await _galaxyService.StarExistsAsync(star))
        {
            return null;
        }
        return await _starVisitRepository.GetActiveFor(new Spaceship
        {
            OwnerId = ownerId
        }, star);
    }
}