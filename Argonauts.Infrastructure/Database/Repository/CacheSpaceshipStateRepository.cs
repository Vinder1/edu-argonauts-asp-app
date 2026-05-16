using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Infrastructure.Database.Repository.DataSources;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
public class CacheSpaceshipStateRepository(ICacheService cacheService) : ISpaceshipStateRepository
{
    private readonly ICacheService _cacheService = cacheService
        ?? throw new ArgumentNullException(nameof(cacheService));

    private static string GetStateKeyFor(Guid playerId) => $"user:{playerId}:state";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<SpaceshipState> GetStateAsync(Guid playerId)
    {
        var state = await _cacheService.GetAsync(GetStateKeyFor(playerId));
        return state switch
        {
            null => SpaceshipState.None,
            "moving" => SpaceshipState.Moving,
            "explore" => SpaceshipState.Exploring,
            _ => SpaceshipState.None
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetStateAsync(Guid playerId, SpaceshipState value, TimeSpan expiry)
    {
        var stateString = value switch
        {
            SpaceshipState.Moving => "moving",
            SpaceshipState.Exploring => "explore",
            SpaceshipState.Battling => "battle",
            _ => ""
        };
        if (stateString == "")
        {
            await _cacheService.RemoveAsync(GetStateKeyFor(playerId));
            return;
        }
        await _cacheService.SetAsync(GetStateKeyFor(playerId), stateString, expiry);
    }
}