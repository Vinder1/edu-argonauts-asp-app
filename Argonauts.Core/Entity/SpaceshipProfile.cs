namespace Argonauts.Core.Entity;

/// <summary>
/// 
/// </summary>
public class SpaceshipProfile
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int BattlePower { get; set; }
}