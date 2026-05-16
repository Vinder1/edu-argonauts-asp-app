using Argonauts.Application.Dto;
using Argonauts.Application.External;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Battle;
using Argonauts.Core.Utility;
using Argonauts.Core.Utility.Content;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="battleService"></param>
/// <param name="spaceshipService"></param>
/// <param name="playerService"></param>
/// <param name="spaceshipConditionService"></param>
/// <param name="spaceshipStateService"></param>
/// <param name="serverEventService"></param>
/// <param name="serviceScopeFactory"></param>
/// <param name="explorationService"></param>
/// <param name="dataContainer"></param>
/// <param name="questService"></param>
public class BattleProcessService(
    IBattleEntityService battleService,
    ISpaceshipService spaceshipService,
    IPlayerService playerService,
    ISpaceshipConditionService spaceshipConditionService,
    ISpaceshipStateRepository spaceshipStateService,
    IServerEventService serverEventService,
    IExplorationService explorationService,
    DataContainer dataContainer,
    IBackgroundScheduler backgroundScheduler,
    IQuestService questService,
    IDestroySpaceshipService destroyService
) : IBattleProcessService
{
    private readonly IBattleEntityService _battleService = battleService
        ?? throw new ArgumentNullException(nameof(battleService));
    private readonly ISpaceshipService _spaceshipService = spaceshipService
        ?? throw new ArgumentNullException(nameof(spaceshipService));
    private readonly IPlayerService _playerService = playerService
        ?? throw new ArgumentNullException(nameof(playerService));
    private readonly ISpaceshipConditionService _spaceshipConditionService = spaceshipConditionService
        ?? throw new ArgumentNullException(nameof(spaceshipConditionService));
    private readonly ISpaceshipStateRepository _spaceshipStateService = spaceshipStateService
        ?? throw new ArgumentNullException(nameof(spaceshipStateService));
    private readonly IServerEventService _serverEventService = serverEventService
        ?? throw new ArgumentNullException(nameof(serverEventService));
    private readonly IExplorationService _explorationService = explorationService
        ?? throw new ArgumentNullException(nameof(explorationService));
    private readonly DataContainer _dataContainer = dataContainer
        ?? throw new ArgumentNullException(nameof(dataContainer));
    private readonly IBackgroundScheduler _backgroundScheduler = backgroundScheduler
        ?? throw new ArgumentNullException(nameof(backgroundScheduler));
    private readonly IQuestService _questService = questService
        ?? throw new ArgumentNullException(nameof(questService));
    private readonly IDestroySpaceshipService _destroyService = destroyService
        ?? throw new ArgumentNullException(nameof(destroyService));


    /// <inheritdoc/>
    public async Task<ServiceActionResult> CreatePvPBattle(Guid userId, Guid targetOwnerId)
    {
        var attackerSpaceship = await _spaceshipService.GetSpaceshipAsync(userId);
        if (attackerSpaceship == null)
            return ServiceActionResult.Invalid("Spaceship not found");

        var attackerCondition = await _spaceshipConditionService.GetForUserAsync(userId);
        if (attackerCondition == null)
            return ServiceActionResult.Invalid("Spaceship condition not found");

        var attackerState = await _spaceshipStateService.GetStateAsync(userId);
        if (attackerState != SpaceshipState.None)
            return ServiceActionResult.Invalid("You are busy with something");

        var targetSpaceship = await _spaceshipService.GetSpaceshipAsync(targetOwnerId);
        if (targetSpaceship == null)
            return ServiceActionResult.Invalid("Target spaceship not found");

        if (attackerSpaceship.LocatedRadius != targetSpaceship.LocatedRadius ||
            attackerSpaceship.LocatedAngleMilliradians != targetSpaceship.LocatedAngleMilliradians)
            return ServiceActionResult.Invalid("Target is not on the same star");

        var targetCondition = await _spaceshipConditionService.GetForUserAsync(targetOwnerId);
        if (targetCondition == null)
            return ServiceActionResult.Invalid("Target condition not found");

        var targetState = await _spaceshipStateService.GetStateAsync(targetOwnerId);
        if (targetState != SpaceshipState.None)
            return ServiceActionResult.Invalid("Target is busy");

        var attacker = await _playerService.GetPlayerAsync(userId);
        var target = await _playerService.GetPlayerAsync(targetOwnerId);
        if (attacker == null || target == null)
            return ServiceActionResult.Invalid("Player missing");

        var attackerMember = new BattleMember
        {
            Id = userId,
            BattleId = default,
            Health = attackerCondition.Durability,
            MaxHealth = attackerCondition.MaxDurability,
            Power = attackerCondition.Power,
            IsAI = false,
            Name = attacker.Name,
            Move = ""
        };

        var targetMember = new BattleMember
        {
            Id = targetOwnerId,
            BattleId = default,
            Health = targetCondition.Durability,
            MaxHealth = targetCondition.MaxDurability,
            Power = targetCondition.Power,
            IsAI = false,
            Name = target.Name,
            Move = ""
        };

        var members = new List<BattleMember> { attackerMember };
        var battleId = await _battleService.CreateBattleAsync(userId, members, TimeSpan.FromMinutes(30), BattleType.PvP);
        await _battleService.AddBattleMemberAsync(battleId, targetMember, TimeSpan.FromMinutes(30));
        await _spaceshipStateService.SetStateAsync(userId, SpaceshipState.Battling, TimeSpan.FromMinutes(30));
        await _spaceshipStateService.SetStateAsync(targetOwnerId, SpaceshipState.Battling, TimeSpan.FromMinutes(30));
        await _serverEventService.SendUserBattleStartAsync(userId, battleId);
        await _serverEventService.SendUserBattleStartAsync(targetOwnerId, battleId);

        var state = await _battleService.GetBattleStatusAsync(battleId);
        _backgroundScheduler.ScheduleAsync<BattleProcessService>(
            s => s.ProcessBattleRound(battleId), TimeSpan.FromSeconds(10));

        return ServiceActionResult.Ok();
    }

    /// <inheritdoc/>
    public async Task<ServiceActionResult> CreateBattleFromExploration(Guid playerId)
    {
        var spaceship = await _spaceshipService.GetSpaceshipAsync(playerId);
        if (spaceship == null)
        {
            return ServiceActionResult.Invalid("Spaceship not found");
        }

        var condition = await _spaceshipConditionService.GetForUserAsync(playerId);
        if (condition == null)
        {
            return ServiceActionResult.Invalid("Spaceship condition not found");
        }

        var state = await _spaceshipStateService.GetStateAsync(playerId);
        if (state != SpaceshipState.None)
        {
            return ServiceActionResult.Invalid("You are busy with something");
        }

        var explorationState = await _explorationService.GetExplorationResultAsync(playerId);

        if (explorationState == null)
        {
            return ServiceActionResult.Invalid("You are not in exploration");
        }

        if (explorationState.Enemy == null)
        {
            return ServiceActionResult.Invalid("You have no enemies");
        }

        var player = await _playerService.GetPlayerAsync(playerId);
        if (player == null)
        {
            return ServiceActionResult.Invalid("Player missing");
        }

        var playerMember = new BattleMember
        {
            Id = spaceship.OwnerId,
            BattleId = default,
            Health = condition.Durability,
            MaxHealth = condition.MaxDurability,
            Power = condition.Power,
            IsAI = false,
            Name = player.Name,
            Move = ""
        };

        var enemyMember = BattleEnemyMemberGenerator.CreateFromExplorationEnemy(explorationState.Enemy, dataContainer);

        var members = new List<BattleMember> { playerMember, enemyMember };

        var battleId = await _battleService.CreateBattleAsync(playerId, members, TimeSpan.FromMinutes(30), BattleType.Exploration);
        await _spaceshipStateService.SetStateAsync(playerId, SpaceshipState.Battling, TimeSpan.FromMinutes(30));
        await _serverEventService.SendUserBattleStartAsync(playerId, battleId);
        _backgroundScheduler.ScheduleAsync<BattleProcessService>(
            s => s.ProcessBattleRound(battleId), TimeSpan.FromSeconds(10));

        return ServiceActionResult.Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleId"></param>
    /// <returns></returns>
    public async Task ProcessBattleRound(Guid battleId)
    {
        var battleMembers = await battleService.GetBattleStatusAsync(battleId);
        if (battleMembers.Count == 0)
            return;

        var battleStatus = new BattleStatus { Members = battleMembers };
        battleStatus.ProcessMove();
        await battleService.UpdateBattleAsync(battleId, battleStatus.Members);

        var players = battleStatus.Members.Where(m => !m.IsAI).Select(m => m.Id);
        if (battleStatus.Continue())
        {
            foreach (var playerId in players)
            {
                await _serverEventService.SendUserBattleRoundEndAsync(playerId);
            }
            _backgroundScheduler.ScheduleAsync<BattleProcessService>(
                s => s.ProcessBattleRound(battleId), TimeSpan.FromSeconds(10));
        }
        else
        {
            var battleType = await _battleService.GetBattleTypeAsync(battleId);
            foreach (var playerId in players)
            {
                var member = battleStatus.Members.FirstOrDefault(m => m.Id == playerId);
                if (member == null)
                    continue;

                if (member.Health <= 0)
                {
                    await _destroyService.Destroy(playerId);
                    await _serverEventService.SendUserDeathAsync(playerId);
                }
                else
                {
                    await SaveDamage(playerId, spaceshipConditionService, member);
                    await ProcessBattleEnd(playerId, battleType, battleStatus);
                }

                await _serverEventService.SendUserBattleEndAsync(playerId);
                await _battleService.EndBattleAsync(playerId);
            }
        }
    }

    private static async Task SaveDamage(Guid playerId, ISpaceshipConditionService conditionService, BattleMember member)
    {
        var condition = await conditionService.GetForUserAsync(playerId);
        if (condition == null)
            return;

        condition.Durability = member.Health;
        await conditionService.UpdateAsync(playerId, condition);
    }

    private async Task ProcessBattleEnd(Guid playerId, BattleType battleType, BattleStatus battle)
    {
        if (battleType == BattleType.Exploration)
        {
            var status = await _explorationService.GetExplorationResultAsync(playerId);
            if (status == null)
                return;

            var enemyLevel = status.Enemy?.Level;
            await _explorationService.KillEnemiesAsync(playerId);

            if (enemyLevel.HasValue)
            {
                await _questService.RegisterKillAsync(playerId, enemyLevel.Value);
            }

            status = await _explorationService.GetExplorationResultAsync(playerId);
            await _serverEventService.SendUserExploreResultAsync(playerId, status!);
        }
        else if (battleType == BattleType.PvP)
        {
            // TODO steal money from slain players
        }
    }
}