using Argonauts.Core.Entity.Battle;
using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Utility.Content;

namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class BattleEnemyMemberGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="dataContainer"></param>
    /// <returns></returns>
    public static BattleMember CreateFromExplorationEnemy(Enemy enemy, DataContainer dataContainer)
    {
        var level = enemy.Level;
        var prefab = dataContainer.GetEnemyForLevel(level);
        return new BattleMember
        {
            Id = Guid.NewGuid(),
            BattleId = default,
            Health = prefab.Health,
            MaxHealth = prefab.Health,
            Power = prefab.Power,
            IsAI = true,
            Name = prefab.Name,
            Move = ""
        };
    }
}