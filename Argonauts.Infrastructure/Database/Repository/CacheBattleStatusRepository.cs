using System.Text.Json;
using Argonauts.Core.Entity.Battle;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="cacheService"></param>
public class CacheBattleStatusRepository(ICacheService cacheService) : IBattleStatusRepository
{
    private static string GetKeyFor(Guid battleId) => $"battle:status:{battleId}";
    private static string GetBattleTypeKey(Guid battleId) => $"battle:type:{battleId}";
    private static string GetBattlePlayersKey(Guid battleId) => $"battle:status:{battleId}:players";
    private static string GetPlayerBattleKey(Guid playerId) => $"battle:player:{playerId}:battleId";

    /// <inheritdoc/>
    public async Task<Guid> GetBattleIdForPlayerAsync(Guid playerId)
    {
        var s = await cacheService.GetAsync(GetPlayerBattleKey(playerId));
        if (Guid.TryParse(s, out var guid))
            return guid;
        return default;
    }

    /// <inheritdoc/>
    public async Task<List<BattleMember>> GetForPlayerAsync(Guid playerId)
    {
        var id = await GetBattleIdForPlayerAsync(playerId);
        return await GetAsync(id);
    }

    /// <inheritdoc/>
    public async Task<List<BattleMember>> GetAsync(Guid battleId)
    {
        var members = new List<BattleMember>();
        var hash = await cacheService.HGetAllAsync(GetKeyFor(battleId));
        foreach (var kvp in hash)
        {
            var member = JsonSerializer.Deserialize<BattleMember>(kvp.Value);
            if (member != null)
                members.Add(member);
        }
        return members;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="members"></param>
    /// <param name="expiry"></param>
    /// <param name="battleType"></param>
    /// <returns></returns>
    public async Task<Guid> CreateAsync(Guid playerId, IEnumerable<BattleMember> members, TimeSpan expiry, BattleType battleType = BattleType.Exploration)
    {
        var battleId = Guid.NewGuid();
        var key = GetKeyFor(battleId);
        foreach (var member in members)
        {
            member.BattleId = battleId;
            var json = JsonSerializer.Serialize(member);
            await cacheService.HSetAsync(key, member.Id.ToString(), json);
        }
        await cacheService.SetAsync(GetBattleTypeKey(battleId), battleType.ToString(), expiry);
        await SetPlayerBattleIdAsync(playerId, battleId, expiry);
        return battleId;
    }

    /// <inheritdoc/>
    public async Task DeleteForPlayerAsync(Guid playerId)
    {
        var battleId = await GetBattleIdForPlayerAsync(playerId);
        await RemovePlayerFromBattleAsync(playerId);
        var list = await GetPlayersInBattleAsync(battleId);
        if (list.Count == 0)
        {
            var key = GetKeyFor(battleId);
            var hash = await cacheService.HGetAllAsync(key);
            foreach (var kvp in hash)
            {
                await cacheService.HDelAsync(key, kvp.Key);
            }
            await cacheService.RemoveAsync(key);
            await cacheService.RemoveAsync(GetBattleTypeKey(battleId));
            await cacheService.RemoveAsync(GetBattlePlayersKey(battleId));
        }
    }

    /// <inheritdoc/>
    public async Task AddNewBattleMemberAsync(Guid battleId, BattleMember member, TimeSpan? expiry = null)
    {
        member.BattleId = battleId;
        var key = GetKeyFor(battleId);
        var json = JsonSerializer.Serialize(member);
        await cacheService.HSetAsync(key, member.Id.ToString(), json);
        if (!member.IsAI)
        {
            await SetPlayerBattleIdAsync(member.Id, member.BattleId, expiry);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateBattleMemberAsync(Guid playerId, Guid memberId, BattleMember member)
    {
        var battleId = await GetBattleIdForPlayerAsync(playerId);
        var key = GetKeyFor(battleId);
        var json = JsonSerializer.Serialize(member);
        await cacheService.HSetAsync(key, memberId.ToString(), json);
    }

    /// <inheritdoc/>
    public async Task UpdateBattleAsync(Guid battleId, IEnumerable<BattleMember> members)
    {
        var key = GetKeyFor(battleId);
        foreach (var member in members)
        {
            member.BattleId = battleId;
            var json = JsonSerializer.Serialize(member);
            await cacheService.HSetAsync(key, member.Id.ToString(), json);
        }
    }

    /// <inheritdoc/>
    public async Task SetPlayerBattleIdAsync(Guid playerId, Guid battleId, TimeSpan? expiry = null)
    {
        await cacheService.SetAsync(GetPlayerBattleKey(playerId), battleId.ToString(), expiry);
        await cacheService.SAddAsync(GetBattlePlayersKey(battleId), playerId.ToString());
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetPlayersInBattleAsync(Guid battleId)
    {
        var members = await cacheService.SMembersAsync(GetBattlePlayersKey(battleId));
        return members
            .Select(m => Guid.TryParse(m, out var g) ? g : default)
            .Where(g => g != default)
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<BattleType> GetBattleTypeAsync(Guid battleId)
    {
        var s = await cacheService.GetAsync(GetBattleTypeKey(battleId));
        if (Enum.TryParse<BattleType>(s, out var battleType))
            return battleType;
        return BattleType.Exploration;
    }

    /// <inheritdoc/>
    public async Task RemovePlayerFromBattleAsync(Guid playerId)
    {
        var battleId = await GetBattleIdForPlayerAsync(playerId);
        if (battleId != default)
        {
            await cacheService.SRemAsync(GetBattlePlayersKey(battleId), playerId.ToString());
        }
        await cacheService.RemoveAsync(GetPlayerBattleKey(playerId));
        await cacheService.HDelAsync(GetKeyFor(battleId), playerId.ToString());
    }
}
