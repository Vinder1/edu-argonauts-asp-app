using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class RatingEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/rating").WithTags("Rating");

        group.MapGet("/top10", async (ISpaceshipOnStarService spaceshipOnStarService) =>
        {
            var spaceships = await spaceshipOnStarService.GetTop10Async();
            return Results.Ok(spaceships);
        }).WithSummary("Топ 10 кораблей по боевой мощи");
    }
}
