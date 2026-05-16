using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Utility.Math;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class StarsEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stars").WithTags("Stars");

        group.MapGet("/all", async (IGalaxyService galaxyService) =>
        {
            var stars = await galaxyService.GetAllStarsAsync();
            return Results.Ok(stars);
        });//.RequireAuthorization(AuthPolicies.Any);

        // GET /stars/find?radius=5&angle=1.57
        group.MapGet("/find", async (int radius, int angleMilliradians, IGalaxyService galaxyService) =>
        {
            if (radius < 0)
            {
                return Results.BadRequest("Radius must be positive");
            }

            if (angleMilliradians < 0 || angleMilliradians > Angle.TwoPiMilliradians)
            {
                return Results.BadRequest($"Angle must be between 0 and {Angle.TwoPiMilliradians}");
            }

            var star = await galaxyService.FindStarAsync(radius, angleMilliradians);

            if (star == null)
            {
                return Results.NotFound($"Star (radius {radius}, angle {angleMilliradians}) not found");
            }

            return Results.Ok(star);
        });

        // GET /stars/byRadiusRange?minRadius=1&maxRadius=10
        group.MapGet("/byRadiusRange", async (int minRadius, int maxRadius, IGalaxyService galaxyService) =>
        {
            if (minRadius < 0)
            {
                return Results.BadRequest("minRadius must be non-negative");
            }

            if (maxRadius < minRadius)
            {
                return Results.BadRequest("maxRadius must be >= minRadius");
            }

            var stars = await galaxyService.GetStarsByRadiusRangeAsync(minRadius, maxRadius);
            var starList = stars.ToList();

            return Results.Ok(new
            {
                Stars = starList,
                MinRadius = minRadius,
                MaxRadius = maxRadius,
                TotalCount = starList.Count
            });
        });

        // GET /stars/nearby?radius=5&angleMilliradians=1570&range=2
        group.MapGet("/nearby", async (int radius, int angleMilliradians, int range, IGalaxyService galaxyService) =>
        {
            if (radius < 0)
            {
                return Results.BadRequest("Radius must be positive");
            }

            if (range < 1)
            {
                return Results.BadRequest("Range must be greater than 0");
            }

            var centerStar = await galaxyService.FindStarAsync(radius, angleMilliradians);

            if (centerStar == null)
            {
                return Results.NotFound($"Star (radius {radius}, angle {angleMilliradians}) not found");
            }

            var nearbyStars = await galaxyService.GetStarsNearStarAsync(centerStar.Value, range);

            return Results.Ok(new
            {
                CenterStar = centerStar,
                Range = range,
                NearbyStars = nearbyStars,
                TotalCount = nearbyStars.Count()
            });
        });
    }
}