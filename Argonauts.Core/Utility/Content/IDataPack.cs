using Argonauts.Core.Entity.Exploration;

namespace Argonauts.Core.Utility.Content;

/// <summary>
/// 
/// </summary>
public interface IDataPack
{
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<int, EnemyPrefab> Enemies { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public EnemyPrefab? NewDefaultEnemy { get; }

    /// <summary>
    /// Guides with explanations on game themes.
    /// </summary>
    public Dictionary<string, string> Guides { get; }

    /// <summary>
    /// Quest descriptions keyed by quest level.
    /// </summary>
    public Dictionary<int, string> QuestDescriptions { get; }
}