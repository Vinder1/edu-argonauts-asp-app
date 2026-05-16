using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class HarvestResultGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="exploration"></param>
    /// <returns></returns>
    public static HarvestResult CreateFor(ExplorationStatus exploration)
    {
        var random = new Random();
        var basicValue = 5 * System.Math.Pow(1.2, exploration.Level);
        if (exploration.Enemy != null)
            basicValue *= 4;

        return new HarvestResult
        {
            AddCurrency = random.Next((int) (basicValue * 0.7), (int) (basicValue * 1.3)),
            AddQuants = random.Next((int) (basicValue * 0.7), (int) (basicValue * 1.3))
        };
    }
}