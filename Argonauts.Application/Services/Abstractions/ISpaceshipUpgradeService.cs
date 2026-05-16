using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Player;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ISpaceshipUpgradeService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeHullAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeEngineAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ServiceActionResultWithBody<SpaceshipCondition>> UpgradeBatteryAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<UpgradeCost> GetCostAsync(Guid playerId);
}