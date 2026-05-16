using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// Endpoints for spaceship resources operations.
/// </summary>
public class SpaceshipResourcesEndpoints : ICarterModule
{
    /// <summary>
    /// Adds routes for spaceship resources endpoints.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var consistencyGroup = app.MapGroup("api/consistency").WithTags("Consistency");
        
        consistencyGroup.MapGet("/restore-sp-res", async (
            IConsistencyService consistencyService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            await consistencyService.AddBalanceAndSpaceshipConditions(userId);
            return Results.Ok();
        }).RequireAuthorization();
    }
}
