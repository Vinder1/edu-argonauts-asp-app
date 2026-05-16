using Argonauts.Core.Entity.Battle;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IBattleEntityService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task<List<BattleMember>> GetBattleStatusForUserAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task<List<BattleMember>> GetBattleStatusAsync(Guid battleId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task<BattleType> GetBattleTypeAsync(Guid battleId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="members"></param>
    /// <param name="expiry"></param>
    /// <param name="battleType"></param>
    /// <returns></returns>
    Task<Guid> CreateBattleAsync(Guid playerId, IEnumerable<BattleMember> members, TimeSpan expiry, BattleType battleType = BattleType.Exploration);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task EndBattleAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <param name="member"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task AddBattleMemberAsync(Guid battleId, BattleMember member, TimeSpan? expiry = null);
    /// <summary>
    ///
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="memberId"></param>
    /// <param name="move"></param>
    /// <param name="targetId"></param>
    /// <returns></returns>
    Task UpdateMemberMoveAsync(Guid playerId, Guid memberId, string move, string? targetId = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <param name="members"></param>
    /// <returns></returns>
    Task UpdateBattleAsync(Guid battleId, IEnumerable<BattleMember> members);
}
