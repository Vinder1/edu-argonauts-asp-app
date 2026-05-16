namespace Argonauts.Core.Entity.Battle;

/// <summary>
/// 
/// </summary>
public class BattleMember
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid BattleId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Health { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int MaxHealth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Power { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAI { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///
    /// </summary>
    public string Move { get; set; } = null!;

    /// <summary>
    ///
    /// </summary>
    public Guid? TargetId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Alive() => Health > 0;
}