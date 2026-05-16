using Argonauts.Core.Entity.Movement;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="cacheService"></param>
public class CacheMovementStatusRepository(ICacheService cacheService) : IMovementStatusRepository
{
    private static string GetKeyFor(Guid playerId) => $"movement:status:{playerId}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public async Task<MovementStatus?> GetForPlayerAsync(Guid playerId)
    {
        return await cacheService.GetJsonAsync<MovementStatus>(GetKeyFor(playerId));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="status"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    public async Task CreateAsync(Guid playerId, MovementStatus status, TimeSpan expiry)
    {
        await cacheService.SetJsonAsync(GetKeyFor(playerId), status, expiry);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public async Task RemoveForPlayerAsync(Guid playerId)
    {
        await cacheService.RemoveAsync(GetKeyFor(playerId));
    }
}