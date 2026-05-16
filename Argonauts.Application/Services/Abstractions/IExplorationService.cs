using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IExplorationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    Task<ServiceActionResultWithBody<ArrivalInfo>> StartExplorationAsync(Guid ownerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ExplorationStatus?> GetExplorationResultAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ServiceActionResultWithBody<HarvestResult>> HarvestAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task ClearAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task KillEnemiesAsync(Guid playerId);
}