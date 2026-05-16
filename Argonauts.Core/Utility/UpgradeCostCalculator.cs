namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class UpgradeCostCalculator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int Cost(int level)
        => (int) System.Math.Round(10 * System.Math.Pow(1.4, level));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="powerLevel"></param>
    /// <returns></returns>
    public static int CostForPower(int powerLevel)
        => Cost(powerLevel - DefaultSpaceshipConditionGenerator.CreateDefault().Power);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="durabilityLevel"></param>
    /// <returns></returns>
    public static int CostForDurability(int durabilityLevel)
        => Cost(durabilityLevel - DefaultSpaceshipConditionGenerator.CreateDefault().Durability);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="antimatterLevel"></param>
    /// <returns></returns>
    public static int CostForAntimatter(int antimatterLevel)
        => Cost(antimatterLevel - DefaultSpaceshipConditionGenerator.CreateDefault().Antimatter);
}