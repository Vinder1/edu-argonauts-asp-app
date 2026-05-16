using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class SpaceshipConditionEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var conditionGroup = app.MapGroup("api/condition").WithTags("SpaceshipCondition");
        conditionGroup.MapGet("/show", async (
            ISpaceshipConditionService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var condition = await spaceshipResourcesService.GetForUserAsync(userId);
            if (condition == null)
                return Results.BadRequest();
            return Results.Ok(condition);
        }).RequireAuthorization();

        conditionGroup.MapPost("/restore", async (
            ISpaceshipConditionService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var res = await spaceshipResourcesService.RestoreAsync(userId);
            if (!res.Success)
                return Results.BadRequest(new { error = res.ErrorDescription });
            return Results.Ok();
        }).RequireAuthorization();
        
        conditionGroup.MapPost("/upgrade-hull", async (
            ISpaceshipUpgradeService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var res = await spaceshipResourcesService.UpgradeHullAsync(userId);
            if (!res.Success)
                return Results.BadRequest(new { error = res.ErrorDescription });
            return Results.Ok(res.Body);
        }).RequireAuthorization();

        conditionGroup.MapPost("/upgrade-engine", async (
            ISpaceshipUpgradeService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var res = await spaceshipResourcesService.UpgradeEngineAsync(userId);
            if (!res.Success)
                return Results.BadRequest(new { error = res.ErrorDescription });
            return Results.Ok(res.Body);
        }).RequireAuthorization();

        conditionGroup.MapPost("/upgrade-battery", async (
            ISpaceshipUpgradeService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var res = await spaceshipResourcesService.UpgradeBatteryAsync(userId);
            if (!res.Success)
                return Results.BadRequest(new { error = res.ErrorDescription });
            return Results.Ok();
        }).RequireAuthorization();
        
        conditionGroup.MapGet("/upgrade-cost", async (
            ISpaceshipUpgradeService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var res = await spaceshipResourcesService.GetCostAsync(userId);
            return Results.Ok(res);
        }).RequireAuthorization();
    }
}