using Argonauts.Core.Entity;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ISpaceshipStateRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<SpaceshipState> GetStateAsync(Guid playerId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task SetStateAsync(Guid playerId, SpaceshipState value, TimeSpan expiry);
}