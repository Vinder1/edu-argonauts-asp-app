namespace Argonauts.Infrastructure.Database.Entity;

/// <summary>
/// 
/// </summary>
public class Balance
{
    /// <summary>
    /// 
    /// </summary>
    public Guid OwnerId { get; init; } // FK к Spaceship

    /// <summary>
    /// 
    /// </summary>
    public int Currency { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public int Quants { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Spaceship Spaceship { get; set; } = null!;
}