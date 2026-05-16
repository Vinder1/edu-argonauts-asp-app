namespace Argonauts.Infrastructure.Database.Entity;

/// <summary>
/// 
/// </summary>
public class SpaceshipStarVisit
{
    /// <summary>
    /// 
    /// </summary>
    public Guid SpaceshipId { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public int StarGalaxyVersion { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public int StarRadius { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public int StarAngleMilliradians { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    public Spaceship Spaceship { get; init; } = null!;
    /// <summary>
    /// 
    /// </summary>
    public Star Star { get; init; } = null!;
}