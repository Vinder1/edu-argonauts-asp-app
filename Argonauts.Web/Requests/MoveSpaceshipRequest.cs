namespace Argonauts.Web.Requests;

/// <summary>
/// Represents a request to move a spaceship to new coordinates.
/// </summary>
public class MoveSpaceshipRequest
{
    /// <summary>
    /// The target radius coordinate.
    /// </summary>
    public int NewRadius { get; set; }

    /// <summary>
    /// The target angle coordinate in milliradians.
    /// </summary>
    public int NewAngle { get; set; }
}