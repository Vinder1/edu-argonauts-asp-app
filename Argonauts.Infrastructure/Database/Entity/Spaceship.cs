namespace Argonauts.Infrastructure.Database.Entity;

/// <summary>
/// 
/// </summary>
public class Spaceship
{
    /// <summary>
    /// 
    /// </summary>
    public Guid OwnerId { get; init; } // FK к Player
    /// <summary>
    /// 
    /// </summary>
    public int GalaxyVersion { get; init; } // FK к Galaxy

    ///    
    public int LocatedRadius { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int LocatedAngleMilliradians { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Player? Owner { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public Galaxy? Galaxy { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public Balance? Balance { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public SpaceshipCondition? SpaceshipCondition { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public Quest? Quest { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public ICollection<SpaceshipStarVisit> VisitedStars { get; init; } = [];
}