using Argonauts.Core.Entity.Player;

namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class StatsUpgradeProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static SpaceshipCondition UpgradeHull(SpaceshipCondition condition)
    {
        return new SpaceshipCondition
        {
            MaxDurability = condition.MaxDurability + 1,
            MaxDistance = condition.MaxDistance + 1,
            MaxAntimatter = condition.MaxAntimatter,
            MaxEnergy = condition.MaxEnergy,
            Power = condition.Power,
            Speed = condition.Speed
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static SpaceshipCondition UpgradeEngine(SpaceshipCondition condition)
    {
        return new SpaceshipCondition
        {
            MaxDurability = condition.MaxDurability,
            MaxDistance = condition.MaxDistance,
            MaxAntimatter = condition.MaxAntimatter,
            MaxEnergy = condition.MaxEnergy,
            Power = condition.Power + 1,
            Speed = condition.Speed + 1
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static SpaceshipCondition UpgradeBattery(SpaceshipCondition condition)
    {
        return new SpaceshipCondition
        {
            MaxDurability = condition.MaxDurability,
            MaxDistance = condition.MaxDistance,
            MaxAntimatter = condition.MaxAntimatter + 1,
            MaxEnergy = condition.MaxEnergy + 1,
            Power = condition.Power,
            Speed = condition.Speed
        };
    }
    
}