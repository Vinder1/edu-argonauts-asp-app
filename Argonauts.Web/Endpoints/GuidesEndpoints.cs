using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class GuidesEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/guides").WithTags("Guides");

        group.MapGet("/", (IGuideService guideService) =>
        {
            var guides = guideService.GetAllGuides();
            return Results.Ok(guides.Keys);
        });

        group.MapGet("/{key}", (string key, IGuideService guideService) =>
        {
            var guide = guideService.GetGuide(key);
            if (guide == null)
            {
                return Results.NotFound(new { error = "Guide not found" });
            }
            return Results.Ok(new { key, text = guide });
        });
    }
}
