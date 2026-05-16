using Argonauts.Application.Dto;
using Argonauts.Application.External;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility;
using Argonauts.Core.Utility.Content;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class ExplorationService(
    ISpaceshipRepository spaceshipRepository,
    ISpaceshipStateRepository stateService,
    ILogger<ExplorationService> logger,
    IStarVisitService starVisitService,
    IGalaxyService galaxyService,
    IExplorationStatusRepository explorationStatusRepository,
    IBalanceService balanceService,
    DataContainer dataContainer,
    IBackgroundScheduler backgroundScheduler,
    IServerEventService serverEventService
) : IExplorationService
{
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));
    private readonly ISpaceshipStateRepository _stateService = stateService
        ?? throw new ArgumentNullException(nameof(stateService));
    private readonly ILogger<ExplorationService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    private readonly IStarVisitService _starVisitService = starVisitService
        ?? throw new ArgumentNullException(nameof(starVisitService));
    private readonly IGalaxyService _galaxyService = galaxyService
        ?? throw new ArgumentNullException(nameof(galaxyService));
    private readonly IExplorationStatusRepository _explorationStatusRepository = explorationStatusRepository
        ?? throw new ArgumentNullException(nameof(explorationStatusRepository));
    private readonly IBalanceService _balanceService = balanceService
        ?? throw new ArgumentNullException(nameof(balanceService));
    private readonly IBackgroundScheduler _backgroundScheduler = backgroundScheduler
        ?? throw new ArgumentNullException(nameof(backgroundScheduler));
    private readonly IServerEventService _serverEventService = serverEventService
        ?? throw new ArgumentNullException(nameof(serverEventService));
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ServiceActionResultWithBody<ArrivalInfo>> StartExplorationAsync(Guid ownerId)
    {
        var spaceship = await _spaceshipRepository.GetAsync(ownerId);
        if (spaceship == null)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Spaceship not found");
        }

        var state = await stateService.GetStateAsync(ownerId);
        if (state != SpaceshipState.None)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid(
                state == SpaceshipState.Exploring ? "Already exploring" : "You are already busy with something");
        }

        var star = new Star
        {
            Radius = spaceship.LocatedRadius,
            AngleMilliradians = spaceship.LocatedAngleMilliradians
        };

        var foundStar = await _galaxyService.FindStarAsync(star.Radius, star.AngleMilliradians);
        if (foundStar == null)
        {
            return ServiceActionResultWithBody<ArrivalInfo>
                .Invalid("Star, where the spaceship is located, not found");
        }

        if (!foundStar.Value.ValidForExploration())
        {
            return ServiceActionResultWithBody<ArrivalInfo>
                .Invalid("This star is not suitable for exploration");
        }

        var active = await _starVisitService.GetActiveFor(ownerId, star);
        if (active != null)
        {
            return ServiceActionResultWithBody<ArrivalInfo>
                .Invalid($"You have already been here within 24 hours. {active.Value.VisitedAt.AddDays(1) - DateTime.Now} left");
        }

        var activeState = await GetExplorationResultAsync(ownerId);
        if (activeState != null)
        {
            return ServiceActionResultWithBody<ArrivalInfo>
                .Invalid("You are already in exploration!");
        }

        _logger.LogInformation("Spaceship (owner={UserId}): Started exploration at R={Radius}, A={Angle}",
            ownerId, spaceship!.LocatedRadius, spaceship.LocatedAngleMilliradians);

        var arrivalTime = DateTime.Now.AddSeconds(5);

        _backgroundScheduler.ScheduleAsync<ExplorationService>(
            s => s.SendInfoAboutExplorationResult(spaceship.OwnerId), TimeSpan.FromSeconds(4.5));

        await _stateService.SetStateAsync(ownerId, SpaceshipState.Exploring, expiry: TimeSpan.FromSeconds(5));
        await SendInfoAboutExplorationStart(arrivalTime, spaceship.OwnerId);

        return ServiceActionResultWithBody<ArrivalInfo>.Ok(new() { ArrivalTime = arrivalTime });
    }

    ///
    public async Task SendInfoAboutExplorationStart(DateTime arrivalTime, Guid playerId)
    {
        await _serverEventService.SendUserStartExploreAsync(playerId, arrivalTime);
    }

    ///
    public async Task SendInfoAboutExplorationResult(Guid playerId)
    {
        var sp = await _spaceshipRepository.GetAsync(playerId);
        if (sp == null) return;
        var star = new Star { AngleMilliradians = sp.LocatedAngleMilliradians, Radius = sp.LocatedRadius };

        var result = ExplorationGenerator.Create(star, dataContainer);

        _logger.LogInformation("Spaceship (owner={UserId}): Exploration status: {ExplorationStatus}",
            playerId, result);

        await _explorationStatusRepository.CreateAsync(playerId, result, TimeSpan.FromDays(1));
        await _serverEventService.SendUserExploreResultAsync(playerId, result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public Task<ExplorationStatus?> GetExplorationResultAsync(Guid playerId)
    {
        return _explorationStatusRepository.GetForPlayerAsync(playerId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public async Task<ServiceActionResultWithBody<HarvestResult>> HarvestAsync(Guid playerId)
    {
        var activeState = await GetExplorationResultAsync(playerId);
        if (activeState == null)
        {
            return ServiceActionResultWithBody<HarvestResult>
                .Invalid("You are not in exploration!");
        }
        if (activeState.Enemy != null && activeState.Enemy.Alive)
        {
            return ServiceActionResultWithBody<HarvestResult>
                .Invalid("Enemy still alive!");
        }
        var harvestResult = HarvestResultGenerator.CreateFor(activeState);
        await _balanceService.AddCurrencyAndQuantsAsync(playerId,
            harvestResult.AddCurrency, harvestResult.AddQuants);
        var newBalance = await _balanceService.GetForUserAsync(playerId);

        var starVisitCreateResult = await _starVisitService.Create(playerId);
        await _serverEventService.SendUserExploreEndAsync(playerId, starVisitCreateResult.Success, newBalance!, DateTime.Now);

        await _explorationStatusRepository.RemoveForPlayerAsync(playerId);

        _logger.LogInformation("Spaceship (owner={UserId}): Harvest done", playerId);

        return ServiceActionResultWithBody<HarvestResult>.Ok(harvestResult);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public Task ClearAsync(Guid playerId)
    {
        return _explorationStatusRepository.RemoveForPlayerAsync(playerId);
    }

    /// <inheritdoc/>
    public async Task KillEnemiesAsync(Guid playerId)
    {
        var activeState = await GetExplorationResultAsync(playerId);
        if (activeState?.Enemy == null)
            return;
        activeState.Enemy.Alive = false;
        await _explorationStatusRepository.UpdateAsync(playerId, activeState, TimeSpan.FromDays(1));
    }
}