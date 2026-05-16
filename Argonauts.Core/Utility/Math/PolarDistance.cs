using Argonauts.Core.Entity.Galaxy;

namespace Argonauts.Core.Utility.Math;

/// <summary>
/// 
/// </summary>
public static class PolarDistance
{
    private const double E = 1e-7; //погрешность сравнения
    /// <summary>
    /// Get the distance between between two stars   
    /// </summary>
    /// <param name="star1"></param>
    /// <param name="star2"></param>
    /// <returns></returns>
    public static double GetDistance(Star star1, Star star2) =>
        GetDistance(star1, star2.Radius, star2.AngleMilliradians);

    /// <summary>
    /// Get the distance between between two stars   
    /// </summary>
    /// <param name="star"></param>
    /// <param name="sourceRadius"></param>
    /// <param name="sourceAngleMilliradians"></param>
    /// <returns></returns>
    public static double GetDistance(Star star, int sourceRadius, int sourceAngleMilliradians)
    {
        double r1 = sourceRadius;
        double r2 = star.Radius;
        var angleMilliradians = System.Math.Abs(star.AngleMilliradians - sourceAngleMilliradians);
        var deltaAngle = Angle.MilliradiansToRadians(Angle.NormalizeMilliradians(angleMilliradians));
        deltaAngle = System.Math.Min(deltaAngle, 2 * System.Math.PI - deltaAngle);

        var distance = System.Math.Sqrt(r1 * r1 + r2 * r2 - 2 * r1 * r2 * System.Math.Cos(deltaAngle));
        return distance;
    }
    /// <summary>
    /// Checks if a star is within distance threshold from a source point.
    /// </summary>
    /// <param name="star"></param>
    /// <param name="sourceRadius"></param>
    /// <param name="sourceAngleMilliradians"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public static bool IsWithinDistance(Star star, int sourceRadius, int sourceAngleMilliradians, int maxDistance)
    {
        var distance = GetDistance(star, sourceRadius, sourceAngleMilliradians);
        return distance <= maxDistance + E;
    }
    /// <summary>
    /// Checks if a star is within distance threshold from another star.
    /// </summary>
    /// <param name="star1"></param>
    /// <param name="star2"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public static bool IsWithinDistance(Star star1, Star star2, int maxDistance) =>
        IsWithinDistance(star1, star2.Radius, star2.AngleMilliradians, maxDistance);
}