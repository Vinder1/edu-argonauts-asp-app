using Argonauts.Core.Entity.Battle;

namespace Argonauts.Core.Repository;

/// <summary>
/// 
/// </summary>
public interface IBattleStatusRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<Guid> GetBattleIdForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<List<BattleMember>> GetForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task<List<BattleMember>> GetAsync(Guid battleId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="members"></param>
    /// <param name="expiry"></param>
    /// <param name="battleType"></param>
    /// <returns></returns>
    Task<Guid> CreateAsync(Guid playerId, IEnumerable<BattleMember> members, TimeSpan expiry, BattleType battleType = BattleType.Exploration);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task<BattleType> GetBattleTypeAsync(Guid battleId);
    /// <summary>
    /// Leave the battle, and delete it if nobody left
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task DeleteForPlayerAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <param name="member"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task AddNewBattleMemberAsync(Guid battleId, BattleMember member, TimeSpan? expiry = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="memberId"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    Task UpdateBattleMemberAsync(Guid playerId, Guid memberId, BattleMember member);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    Task UpdateBattleAsync(Guid battleId, IEnumerable<BattleMember> members);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="battleId"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task SetPlayerBattleIdAsync(Guid playerId, Guid battleId, TimeSpan? expiry = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task<List<Guid>> GetPlayersInBattleAsync(Guid battleId);
    /// <summary>
    /// Just leave the battle
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task RemovePlayerFromBattleAsync(Guid playerId);
}
