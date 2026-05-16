using Argonauts.Application.Dto;
using Argonauts.Application.External;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Movement;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility.Math;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;
/// <summary>
/// 
/// </summary>
public class MoveSpaceshipService(
    ISpaceshipRepository spaceshipRepository,
    IGalaxyService galaxyService,
    IScopeFactory serviceScopeFactory,
    ISpaceshipStateRepository stateService,
    ILogger<MoveSpaceshipService> logger,
    IExplorationService explorationService,
    IMovementStatusRepository movementStatusRepository,
    ISpaceshipConditionService spaceshipConditionService,
    IDestroySpaceshipService destroySpaceshipService,
    IConsistencyService consistencyService,
    IServerEventService serverEventService,
    IBackgroundScheduler backgroundScheduler
) : IMoveSpaceshipService
{
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));
    private readonly IGalaxyService _galaxyService = galaxyService
        ?? throw new ArgumentNullException(nameof(galaxyService));
    private readonly IScopeFactory _serviceScopeFactory = serviceScopeFactory
        ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly ISpaceshipStateRepository _stateService = stateService
        ?? throw new ArgumentNullException(nameof(stateService));
    private readonly ILogger<MoveSpaceshipService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    private readonly IExplorationService _explorationService = explorationService
        ?? throw new ArgumentNullException(nameof(explorationService));
    private readonly IMovementStatusRepository _movementStatusRepository = movementStatusRepository
        ?? throw new ArgumentNullException(nameof(movementStatusRepository));
    private readonly ISpaceshipConditionService _spaceshipConditionService = spaceshipConditionService
        ?? throw new ArgumentNullException(nameof(spaceshipConditionService));
    private readonly IDestroySpaceshipService _destroySpaceshipService = destroySpaceshipService
        ?? throw new ArgumentNullException(nameof(destroySpaceshipService));
    private readonly IConsistencyService _consistencyService = consistencyService
        ?? throw new ArgumentNullException(nameof(consistencyService));
    private readonly IServerEventService _serverEventService = serverEventService
        ?? throw new ArgumentNullException(nameof(serverEventService));
    private readonly IBackgroundScheduler _backgroundScheduler = backgroundScheduler
        ?? throw new ArgumentNullException(nameof(backgroundScheduler));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="newStar"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="StarNotFoundException"></exception>
    public async Task<ServiceActionResultWithBody<ArrivalInfo>> MoveSpaceshipAsync(Guid ownerId, Star newStar)
    {
        var newRadius = newStar.Radius;
        var newAngleMilliradians = newStar.AngleMilliradians;

        if (newRadius < 0)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Radius cannot be negative");
        }

        newAngleMilliradians = Angle.NormalizeMilliradians(newAngleMilliradians);

        if (!await _galaxyService.StarExistsAsync(newRadius, newAngleMilliradians))
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Star not found");
        }

        var oldSpaceship = await _spaceshipRepository.GetAsync(ownerId);
        if (oldSpaceship == null)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Spaceship not found");
        }
        if (oldSpaceship.LocatedAngleMilliradians == newAngleMilliradians
            && oldSpaceship.LocatedRadius == newRadius)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Same star, no movement required");
        }

        var state = await stateService.GetStateAsync(ownerId);
        if (state != SpaceshipState.None)
        {
            return ServiceActionResultWithBody<ArrivalInfo>.Invalid(
                state == SpaceshipState.Moving ? "Already moving" : "You are already busy with something");
        }

        var condition = await _spaceshipConditionService.GetForUserAsync(ownerId);
        if (condition == null)
        {
            await _consistencyService.AddBalanceAndSpaceshipConditions(ownerId);
            condition = await _spaceshipConditionService.GetForUserAsync(ownerId);
        }

        var oldStar = new Star { Radius = oldSpaceship.LocatedRadius, AngleMilliradians = oldSpaceship.LocatedAngleMilliradians };

        var distance = PolarDistance.GetDistance(oldStar, newStar);
        if (distance * 10 > condition!.MaxDistance)
        {
            return ServiceActionResultWithBody<ArrivalInfo>
                .Invalid($"Target star is too far away from you ({distance}, max: {(double)condition.MaxDistance / 10})");
        }

        if (condition!.Energy > 0)
        {
            condition.Energy -= 1;
        }
        else
        {
            var durabilityLoss = Math.Max(1, condition.MaxDurability / 10);
            condition.Durability -= durabilityLoss;

            if (condition.Durability <= 0)
            {
                await _destroySpaceshipService.Destroy(ownerId);
                await _serverEventService.SendUserDeathAsync(ownerId);
                return ServiceActionResultWithBody<ArrivalInfo>.Invalid("Ship destroyed due to lack of energy. Be careful next time!)");
            }
        }

        await _spaceshipConditionService.UpdateAsync(ownerId, condition);

        await _spaceshipRepository.MoveAsync(ownerId, newRadius, newAngleMilliradians);
        var updatedShip = await _spaceshipRepository.GetAsync(ownerId);

        _logger.LogInformation("Spaceship (owner={UserId}): Moved to R={Radius}, A={Angle}",
            ownerId, updatedShip!.LocatedRadius, updatedShip.LocatedAngleMilliradians);

        var movingTime = distance*10 / Math.Min(condition.Speed, 3);

        _backgroundScheduler.ScheduleAsync<MoveSpaceshipService>(
            s => s.SendInfoAboutSpaceshipCome(updatedShip), TimeSpan.FromSeconds(movingTime));

        await _stateService.SetStateAsync(ownerId, SpaceshipState.Moving, expiry: TimeSpan.FromSeconds(movingTime));
        await _explorationService.ClearAsync(ownerId);
        
        var arrivalTime = DateTime.Now.AddSeconds(movingTime);
        var destinationStar = new Star { AngleMilliradians = newAngleMilliradians, Radius = newRadius };
        await SendInfoAboutSpaceshipStarMove(arrivalTime, oldSpaceship, destinationStar);

        var movementStatus = new MovementStatus
        {
            Started = DateTime.Now,
            Ends = arrivalTime,
            From = oldStar,
            To = newStar
        };
        await _movementStatusRepository.CreateAsync(ownerId, movementStatus, TimeSpan.FromSeconds(movingTime));

        return ServiceActionResultWithBody<ArrivalInfo>.Ok(new() { ArrivalTime = arrivalTime });
    }

    ///
    public async Task SendInfoAboutSpaceshipStarMove(DateTime arrivalTime, Spaceship oldSpaceship, Star destination)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _serverEventService = scope.Resolve<IServerEventService>();
        await _serverEventService.SendUserStartMoveAsync(oldSpaceship.OwnerId, arrivalTime, destination);
        await _serverEventService.SendLocIncomeShipAsync(oldSpaceship.LocatedRadius, oldSpaceship.LocatedAngleMilliradians);
    }

    ///
    public async Task SendInfoAboutSpaceshipCome(Spaceship spaceship)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var _serverEventService = scope.Resolve<IServerEventService>();
        await _serverEventService.SendUserConfirmMoveAsync(spaceship.OwnerId, spaceship.LocatedRadius, spaceship.LocatedAngleMilliradians);
        await _serverEventService.SendLocIncomeShipAsync(spaceship.LocatedRadius, spaceship.LocatedAngleMilliradians);
        await _movementStatusRepository.RemoveForPlayerAsync(spaceship.OwnerId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public Task<MovementStatus?> GetStatusAsync(Guid playerId)
    {
        return _movementStatusRepository.GetForPlayerAsync(playerId);
    }
}