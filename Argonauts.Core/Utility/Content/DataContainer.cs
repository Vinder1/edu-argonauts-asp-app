using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Core.Utility.Content;

/// <summary>
/// 
/// </summary>
public class DataContainer
{
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<int, EnemyPrefab> Enemies { get; } = [];
    /// <summary>
    /// 
    /// </summary>
    public EnemyPrefab? DefaultEnemy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> Guides { get; } = [];

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<int, string> QuestDescriptions { get; } = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPack"></param>
    public void AddDataPack(IDataPack dataPack)
    {
        foreach (var (key,value) in dataPack.Enemies)
        {
            Enemies[key] = value;
        }
        if (dataPack.NewDefaultEnemy != null)
        {
            DefaultEnemy = dataPack.NewDefaultEnemy;
        }
        foreach (var (key, value) in dataPack.Guides)
        {
            Guides[key] = value;
        }
        foreach (var (level, description) in dataPack.QuestDescriptions)
        {
            QuestDescriptions[level] = description;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public EnemyPrefab GetEnemyForLevel(int level)
    {
        if (Enemies.TryGetValue(level, out var enemy))
            return enemy;
        if (DefaultEnemy != null)
            return DefaultEnemy;
        throw new Exception("Enemy not defined");
    }
}