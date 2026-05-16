using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Battle;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="battleStatusRepository"></param>
public class BattleEntityService(IBattleStatusRepository battleStatusRepository) : IBattleEntityService
{
    private readonly IBattleStatusRepository _battleStatusRepository = battleStatusRepository;

    /// <inheritdoc/>
    public Task<List<BattleMember>> GetBattleStatusForUserAsync(Guid playerId)
    {
        return _battleStatusRepository.GetForPlayerAsync(playerId);
    }
    
    /// <inheritdoc/>
    public Task<List<BattleMember>> GetBattleStatusAsync(Guid battleId)
    {
        return _battleStatusRepository.GetAsync(battleId);
    }

    /// <inheritdoc/>
    public Task<Guid> CreateBattleAsync(Guid playerId, IEnumerable<BattleMember> members, TimeSpan expiry, BattleType battleType = BattleType.None)
    {
        return _battleStatusRepository.CreateAsync(playerId, members, expiry, battleType);
    }

    /// <inheritdoc/>
    public Task EndBattleAsync(Guid playerId)
    {
        return _battleStatusRepository.DeleteForPlayerAsync(playerId);
    }

    /// <inheritdoc/>
    public Task AddBattleMemberAsync(Guid battleId, BattleMember member, TimeSpan? expiry = null)
    {
        return _battleStatusRepository.AddNewBattleMemberAsync(battleId, member, expiry);
    }
    
    /// <inheritdoc/>
    public async Task UpdateMemberMoveAsync(Guid playerId, Guid memberId, string move, string? targetId = null)
    {
        var members = await _battleStatusRepository.GetForPlayerAsync(playerId);
        var member = members.FirstOrDefault(m => m.Id == memberId);
        if (member == null) return;

        member.Move = move;
        if (!string.IsNullOrEmpty(targetId) && Guid.TryParse(targetId, out var targetGuid))
        {
            member.TargetId = targetGuid;
        }
        await _battleStatusRepository.UpdateBattleMemberAsync(playerId, memberId, member);
    }

    /// <inheritdoc/>
    public Task UpdateBattleAsync(Guid battleId, IEnumerable<BattleMember> members)
    {
        return _battleStatusRepository.UpdateBattleAsync(battleId, members);
    }

    /// <inheritdoc/>
    public Task<BattleType> GetBattleTypeAsync(Guid battleId)
    {
        return _battleStatusRepository.GetBattleTypeAsync(battleId);
    }

}
