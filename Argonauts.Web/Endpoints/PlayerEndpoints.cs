using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Web.Auth;
using Argonauts.Web.Mapping;
using Argonauts.Web.Requests;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class PlayerEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/player").WithTags("Player");
        
        group.MapGet("/", async (
            [FromServices] IPlayerService playerService,
            [FromServices] AppToEndpointsMapper mapper,
            ClaimsPrincipal userClaim) =>
        {
            var idUnparsed = userClaim.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var id = Guid.Parse(idUnparsed);

            var user = await playerService.GetPlayerAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(mapper.ToUserDto(user));

        }).RequireAuthorization(AuthPolicies.Any);

        group.MapGet("/find", async ([FromServices] IPlayerService playerService, Guid id) =>
        {
            var player = await playerService.GetPlayerAsync(id);
            if (player == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(new
            {
                player.Id,
                player.Name
            });
        });

        group.MapPost("/new-name", async (
            [FromServices] IPlayerService playerService,
            IValidator<UpdatePlayerNameRequest> validator,
            UpdatePlayerNameRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var player = await playerService.UpdatePlayerNameAsync(request.Id, request.Name);
            return Results.Ok(new
            {
                player.Id,
                newName = player.Name
            });
        }).RequireAuthorization(AuthPolicies.Any);

        group.MapDelete("/", async ([FromServices] IPlayerService playerService, Guid id) =>
        {
            await playerService.DeletePlayerAsync(id);
            return Results.Ok();
        });

        group.MapGet("/exists", async ([FromServices] IPlayerService playerService, Guid id) =>
        {
            return Results.Ok(new
            {
                exists = await playerService.PlayerExistsAsync(id)
            });
        });
    }
}