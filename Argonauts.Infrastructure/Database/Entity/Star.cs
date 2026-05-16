namespace Argonauts.Infrastructure.Database.Entity;

/// <summary>
/// 
/// </summary>
public class Star
{
    // Composite Key (gv,r,a)

    /// <summary>
    /// 
    /// </summary>
    public int GalaxyVersion { get; set; } // FK к Galaxy
    /// <summary>
    /// 
    /// </summary>
    public int Radius { get; init; }
    /// <summary>
    /// 
    /// </summary>
    public int AngleMilliradians  { get; init; } // 0-6283
    
    /// <summary>
    /// 
    /// </summary>
    public Galaxy Galaxy { get; init; } = null!;
    
    /// <summary>
    /// 
    /// </summary>
    public string Type { get; set; } = "-";

    /// <summary>
    /// 
    /// </summary>
    public ICollection<SpaceshipStarVisit> VisitedByShips { get; init; } = [];
}