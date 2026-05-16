using Argonauts.Application.Dto;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class SpaceshipUpgradeService(
    ISpaceshipConditionRepository spaceshipConditionRepository,
    IBalanceService balanceService,
    ISpaceshipService spaceshipService,
    IGalaxyService galaxyService
) : ISpaceshipUpgradeService
{
    private readonly ISpaceshipConditionRepository _spaceshipConditionRepository = spaceshipConditionRepository
        ?? throw new ArgumentNullException(nameof(spaceshipConditionRepository));
    private readonly IBalanceService _balanceService = balanceService
        ?? throw new ArgumentNullException(nameof(balanceService));
    private readonly ISpaceshipService _spaceshipService = spaceshipService
        ?? throw new ArgumentNullException(nameof(spaceshipService));
    private readonly IGalaxyService _galaxyService = galaxyService
        ?? throw new ArgumentNullException(nameof(galaxyService));

    private async Task<ServiceActionResultWithBody<SpaceshipCondition>> CheckBasicConditions(Guid playerId)
    {
        var spaceship = await _spaceshipService.GetSpaceshipAsync(playerId);
        if (spaceship == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Spaceship is not found");
        }
        var star = await _galaxyService.FindStarAsync(spaceship.LocatedRadius, spaceship.LocatedAngleMilliradians);
        if (star == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Star, where spaceship is located, is not found");
        }
        if (star.Value.Type != "hub")
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("You can only upgrade in hubs!");
        }
        var condition = await _spaceshipConditionRepository.GetForUserAsync(playerId);
        if (condition == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Condition not found");
        }
        return ServiceActionResultWithBody<SpaceshipCondition>.Ok(condition);
    }

    /// <inheritdoc/>
    public async Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeBatteryAsync(Guid playerId)
    {
        var preRes = await CheckBasicConditions(playerId);
        if (!preRes.Success)
            return preRes;
        var condition = preRes.Body!;
        var cost = UpgradeCostCalculator.CostForAntimatter(condition.MaxAntimatter);
        var balance = await _balanceService.GetForUserAsync(playerId);

        if (balance == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Balance not found");
        }
        if (balance.Quants < cost)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("You don't have enough quarts. Come back when you become richer!");
        }

        var newStats = StatsUpgradeProvider.UpgradeBattery(condition);
        await _spaceshipConditionRepository.UpdateMaxAsync(playerId, newStats);

        balance.Quants -= cost;
        await _balanceService.AddQuantsAsync(playerId, -cost);

        return ServiceActionResultWithBody<SpaceshipCondition>.Ok(newStats);
    }

    /// <inheritdoc/>
    public async Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeEngineAsync(Guid playerId)
    {
        var preRes = await CheckBasicConditions(playerId);
        if (!preRes.Success)
            return preRes;
        var condition = preRes.Body!;
        var cost = UpgradeCostCalculator.CostForPower(condition.Power);
        var balance = await _balanceService.GetForUserAsync(playerId);

        if (balance == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Balance not found");
        }
        if (balance.Quants < cost)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("You don't have enough quarts. Come back when you become richer!");
        }

        var newStats = StatsUpgradeProvider.UpgradeEngine(condition);
        await _spaceshipConditionRepository.UpdateMaxAsync(playerId, newStats);

        balance.Quants -= cost;
        await _balanceService.AddQuantsAsync(playerId, -cost);

        return ServiceActionResultWithBody<SpaceshipCondition>.Ok(newStats);
    }

    /// <inheritdoc/>
    public async Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeHullAsync(Guid playerId)
    {
        var preRes = await CheckBasicConditions(playerId);
        if (!preRes.Success)
            return preRes;
        var condition = preRes.Body!;
        var cost = UpgradeCostCalculator.CostForDurability(condition.MaxDurability);
        var balance = await _balanceService.GetForUserAsync(playerId);

        if (balance == null)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("Balance not found");
        }
        if (balance.Quants < cost)
        {
            return ServiceActionResultWithBody<SpaceshipCondition>
                .Invalid("You don't have enough quarts. Come back when you become richer!");
        }

        var newStats = StatsUpgradeProvider.UpgradeHull(condition);
        await _spaceshipConditionRepository.UpdateMaxAsync(playerId, newStats);

        balance.Quants -= cost;
        await _balanceService.AddQuantsAsync(playerId, -cost);

        return ServiceActionResultWithBody<SpaceshipCondition>.Ok(newStats);
    }

    /// <inheritdoc/>
    public async Task<UpgradeCost> GetCostAsync(Guid playerId)
    {
        var condition = await _spaceshipConditionRepository.GetForUserAsync(playerId);
        if (condition == null)
        {
            return new UpgradeCost
            {
                Battery = 10,
                Engine = 10,
                Hull = 10
            };
        }
        return new UpgradeCost
        {
            Battery = UpgradeCostCalculator.CostForAntimatter(condition.MaxAntimatter),
            Engine = UpgradeCostCalculator.CostForPower(condition.Power),
            Hull = UpgradeCostCalculator.CostForDurability(condition.MaxDurability)
        };
    }
}