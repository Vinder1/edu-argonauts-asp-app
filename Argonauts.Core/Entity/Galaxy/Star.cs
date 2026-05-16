namespace Argonauts.Core.Entity.Galaxy;

/// <summary>
/// Represents a star entity with coordinate information and visit tracking.
/// </summary>
public readonly struct Star
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Star() {}

    /// <summary>
    /// The radius coordinate of the star.
    /// </summary>
    public int Radius { get; init; }

    /// <summary>
    /// The angle coordinate in milliradians (0-6283).
    /// </summary>
    public int AngleMilliradians { get; init; }

    /// <summary>
    /// The type of the star (like "hub", "raid", etc.). Default is "-".
    /// </summary>
    public string Type { get; init; } = "-";

    /// <summary>
    ///   
    /// </summary>
    /// <returns>true if Type is null or empty or '-', otherwise false</returns>
    public bool ValidForExploration() => string.IsNullOrEmpty(Type) || Type == "-";
}