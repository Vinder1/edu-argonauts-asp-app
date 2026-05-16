using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="cacheService"></param>
public class CacheExplorationStatusRepository(ICacheService cacheService) : IExplorationStatusRepository
{
    private static string GetKeyFor(Guid playerId) => $"exploration:result:{playerId}";

    /// <inheritdoc/>
    public Task<ExplorationStatus?> GetForPlayerAsync(Guid playerId)
    {
        return cacheService.GetJsonAsync<ExplorationStatus>(GetKeyFor(playerId));
    }

    /// <inheritdoc/>
    public Task CreateAsync(Guid playerId, ExplorationStatus result, TimeSpan expiry)
    {
        return cacheService.SetJsonAsync(GetKeyFor(playerId), result, expiry);
    }

    /// <inheritdoc/>
    public Task RemoveForPlayerAsync(Guid playerId)
    {
        return cacheService.RemoveAsync(GetKeyFor(playerId));
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Guid playerId, ExplorationStatus status, TimeSpan expiry)
    {
        return cacheService.SetJsonAsync(GetKeyFor(playerId), status, expiry);
    }
}