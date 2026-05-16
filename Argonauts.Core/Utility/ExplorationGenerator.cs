using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Utility.Content;

namespace Argonauts.Core.Utility;

/// <summary>
/// 
/// </summary>
public static class ExplorationGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    /// <param name="dataContainer"></param>
    /// <returns></returns>
    public static ExplorationStatus Create(Star star, DataContainer dataContainer)
    {
        if (Random.Shared.Next() % 100 < 70)
        {
            var level = (star.Radius - 1) / 5 + 1;
            var prefab = dataContainer.GetEnemyForLevel(level);
            return new ExplorationStatus
            {
                Level = star.Radius,
                Enemy = new Enemy { Level = level, Name = prefab.Name }
            };
        }
        return new ExplorationStatus
        {
            Level = star.Radius
        };
    }
}