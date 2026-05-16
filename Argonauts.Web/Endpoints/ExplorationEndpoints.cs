using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class ExplorationEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/explore").WithTags("Exploration");

        group.MapPost("/start",
            async (IExplorationService explorationService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var result = await explorationService.StartExplorationAsync(ownerId);
            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.ErrorDescription });
            }
            return Results.Ok(result.Body);
        });
        
        group.MapGet("/state",
            async (IExplorationService explorationService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var result = await explorationService.GetExplorationResultAsync(ownerId);
            if (result == null)
            {
                return Results.BadRequest();
            }
            return Results.Ok(result);
        });
        
        group.MapPost("/harvest",
            async (IExplorationService explorationService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var result = await explorationService.HarvestAsync(ownerId);
            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.ErrorDescription });
            }
            return Results.Ok(result.Body);
        });
    }
}