using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Core.Utility.Content;

/// <summary>
/// 
/// </summary>
public static class DataPackBuildingExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPack"></param>
    /// <param name="level"></param>
    /// <param name="health"></param>
    /// <param name="power"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IDataPack AddEnemy(this IDataPack dataPack, int level, int health, int power, string name)
    {
        dataPack.Enemies[level] = new EnemyPrefab
        {
            Level = level,
            Health = health,
            Power = power,
            Name = name
        };
        return dataPack;
    }
}