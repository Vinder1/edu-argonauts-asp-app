namespace Argonauts.Core.Entity.Exploration;

/// <summary>
/// 
/// </summary>
public class Enemy
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool Alive { get; set; } = true;
}