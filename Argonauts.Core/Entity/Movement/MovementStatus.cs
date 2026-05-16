using Argonauts.Core.Entity.Galaxy;

namespace Argonauts.Core.Entity.Movement;

/// <summary>
/// 
/// </summary>
public class MovementStatus
{
    /// <summary>
    /// 
    /// </summary>
    public DateTime Started { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Ends { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Star From { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Star To { get; set; }
}