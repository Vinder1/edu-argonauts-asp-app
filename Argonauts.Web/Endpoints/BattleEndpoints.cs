using System.Security.Claims;
using Argonauts.Application.External;
using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class BattleEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/battle").WithTags("Battle");

        group.MapPost("/attack-player",
            async (IBattleProcessService createBattleService, ClaimsPrincipal user, Guid targetOwnerId) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var result = await createBattleService.CreatePvPBattle(ownerId, targetOwnerId);
            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.ErrorDescription });
            }
            return Results.Ok(new { message = "Battle created successfully" });
        });

        group.MapPost("/create",
            async (IBattleProcessService createBattleService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var result = await createBattleService.CreateBattleFromExploration(ownerId);
            if (!result.Success)
            {
                return Results.BadRequest(new { error = result.ErrorDescription });
            }
            return Results.Ok(new { message = "Battle created successfully" });
        });

        group.MapGet("/state",
            async (IBattleEntityService battleService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var battleState = await battleService.GetBattleStatusForUserAsync(ownerId);
            if (battleState == null || battleState.Count == 0)
            {
                return Results.NotFound(new { error = "No active battle found" });
            }
            return Results.Ok(battleState);
        });

        group.MapPost("/move",
            async (IBattleEntityService battleService, ClaimsPrincipal user, string move, string? targetId) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            await battleService.UpdateMemberMoveAsync(ownerId, ownerId, move, targetId);
            return Results.Ok(new { message = "Move updated" });
        });

        group.MapPost("/end",
            async (IBattleEntityService battleService, IServerEventService serverEventService, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            await battleService.EndBattleAsync(ownerId);
            await serverEventService.SendUserBattleEndAsync(ownerId);
            return Results.Ok(new { message = "Battle ended" });
        });
    }
}