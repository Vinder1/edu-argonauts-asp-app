using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Core.Repository;

/// <summary>
/// 
/// </summary>
public interface IExplorationStatusRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<ExplorationStatus?> GetForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="result"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task CreateAsync(Guid playerId, ExplorationStatus result, TimeSpan expiry);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task RemoveForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="status"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task UpdateAsync(Guid playerId, ExplorationStatus status, TimeSpan expiry);
}