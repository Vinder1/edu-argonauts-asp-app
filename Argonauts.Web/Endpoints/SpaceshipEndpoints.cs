using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Web.Auth;
using Argonauts.Web.Mapping;
using Argonauts.Web.Requests;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class SpaceshipEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/spaceship").WithTags("Spaceship");

        group.MapPost("/create",
            async (ISpaceshipService spaceshipService, IGalaxyService galaxyService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var res = await spaceshipService.CreateSpaceshipAsync(ownerId);
            if (!res.Success)
            {
                return Results.BadRequest(res.ErrorDescription);
            }
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.Any)
        .WithSummary("Инициализация корабля");

        group.MapGet("/", async (
            ISpaceshipService spaceshipService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var spaceship = await spaceshipService.GetSpaceshipAsync(ownerId);
            if (spaceship == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(spaceship);
        });
        
        group.MapGet("/state", async (ISpaceshipStateRepository stateService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);
            var state = await stateService.GetStateAsync(ownerId);
            return Results.Ok(state);
        }).RequireAuthorization(AuthPolicies.Any);

        group.MapGet("/find",
            async (ISpaceshipService spaceshipService, [FromQuery] Guid owner) =>
        {
            var spaceship = await spaceshipService.GetSpaceshipAsync(owner);
            if (spaceship == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(new
            {
                spaceship.OwnerId,
                spaceship.LocatedAngleMilliradians,
                spaceship.LocatedRadius
            });
        });

        group.MapGet("/all", async (ISpaceshipService spaceshipService) =>
        {
            var spaceships = await spaceshipService.GetAllSpaceshipsAsync();
            return Results.Ok(spaceships);
        });
        
        group.MapGet("/all-on-star", async (ISpaceshipOnStarService spaceshipOnStarService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var spaceships = await spaceshipOnStarService.GetOnStarWhereUserStands(ownerId);
            return Results.Ok(spaceships);
        }).RequireAuthorization(AuthPolicies.Any);

        group.MapPost("/move",
            async (IMoveSpaceshipService moveSpaceshipService,
            [FromServices] AppToEndpointsMapper mapper,
            ClaimsPrincipal user,
            [FromBody] MoveSpaceshipRequest request) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var star = mapper.ToStarFromMovementRequest(request);
            var result = await moveSpaceshipService.MoveSpaceshipAsync(ownerId, star);
            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.ErrorDescription });
            }
            return Results.Ok(result.Body);
        });

        group.MapGet("/movement-status",
            async (IMoveSpaceshipService moveSpaceshipService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var status = await moveSpaceshipService.GetStatusAsync(ownerId);
            return Results.Ok(status);
        });

        // Admin only!
        group.MapDelete("/clean", async (ISpaceshipService spaceshipService) =>
        {
            await spaceshipService.CleanupInactiveSpaceshipsAsync();
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);
    }
}