namespace Argonauts.Infrastructure.Database.Entity;

/// <summary>
/// 
/// </summary>
public class SpaceshipCondition
{
    /// <summary>
    /// 
    /// </summary>
    public Guid OwnerId { get; init; } // FK к Spaceship

    /// <summary>
    /// 
    /// </summary>
    public int MaxDurability { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Durability { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxEnergy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Energy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxAntimatter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Antimatter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Power { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int MaxDistance { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Spaceship Spaceship { get; set; } = null!;
}