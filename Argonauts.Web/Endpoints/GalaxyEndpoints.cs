using Argonauts.Application.Services.Abstractions;
using Argonauts.Web.Auth;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class GalaxyEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/galaxy").WithTags("Galaxy");

        // Only admins must have access to this endpoint
        group.MapPost("/regenerate", async (IGalaxyService galaxyService, ISpaceshipService spaceshipService) =>
        {
            await galaxyService.RegenerateGalaxyAsync();
            await spaceshipService.CleanupInactiveSpaceshipsAsync();
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);

        group.MapGet("/", async (IGalaxyService galaxyService) =>
        {
            var version = await galaxyService.GetCurrentGalaxyVersionAsync();
            return Results.Ok(new
            {
                version
            });
        });
    }
}