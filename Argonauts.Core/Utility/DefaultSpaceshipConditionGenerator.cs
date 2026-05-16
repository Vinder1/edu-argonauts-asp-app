using Argonauts.Core.Entity.Player;

namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class DefaultSpaceshipConditionGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static SpaceshipCondition CreateDefault() => new ()
    {
        MaxDurability = 25,
        MaxEnergy = 30,
        MaxAntimatter = 5,
        Power = 25,
        MaxDistance = 25,
        Speed = 3,

        Durability = 25,
        Energy = 30,
        Antimatter = 5
    };
}