using Argonauts.Core.Entity.Movement;

namespace Argonauts.Core.Repository;

/// <summary>
/// 
/// </summary>
public interface IMovementStatusRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<MovementStatus?> GetForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="status"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task CreateAsync(Guid playerId, MovementStatus status, TimeSpan expiry);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task RemoveForPlayerAsync(Guid playerId);
}