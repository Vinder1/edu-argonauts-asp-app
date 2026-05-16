using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Web.Requests;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class StarVisitEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/star-visit").WithTags("Star Visit");

        // group.MapPost("/create", async (
        //     // [FromBody] StarVisitRequest request,
        //     ClaimsPrincipal user,
        //     [FromServices] AppToEndpointsMapper mapper,
        //     IStarVisitService starVisitService) =>
        // {
        //     var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     if (idUnparsed == null)
        //     {
        //         return Results.Unauthorized();
        //     }
        //     var ownerId = Guid.Parse(idUnparsed);

        //     var res = await starVisitService.Create(ownerId);
        //     if (!res.Success)
        //     {
        //         return Results.BadRequest(res.ErrorDescription);
        //     }
        //     return Results.Ok(new { message = "Visit created successfully" });
        // })
        // .RequireAuthorization(AuthPolicies.Any)
        // .WithName("CreateStarVisit")
        // .WithDescription("Creates a spaceship visit to a star")
        // .Produces(200)
        // .Produces(400)
        // .Produces(404);

        // Получает активный визит (не старше 1 дня)
        group.MapGet("/active", async (
            [FromQuery] int radius,
            [FromQuery] int angleMilliradians,
            ClaimsPrincipal user,
            IStarVisitService starVisitService) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var ownerId = Guid.Parse(idUnparsed);

            var star = new Star
            {
                Radius = radius,
                AngleMilliradians = angleMilliradians
            };

            var visit = await starVisitService.GetActiveFor(ownerId, star);

            if (visit == null)
                return Results.Ok(new StarVisitResponse { Exists = false });

            return Results.Ok(new StarVisitResponse
            {
                Exists = true,
                VisitedAt = visit.Value.VisitedAt,
            });
        })
        .WithName("GetActiveVisit")
        .WithDescription("Gets the active visit (not older than 1 day)")
        .Produces<StarVisitResponse>(200)
        .Produces(401)
        .Produces<StarVisitResponse>(404);
    }
}