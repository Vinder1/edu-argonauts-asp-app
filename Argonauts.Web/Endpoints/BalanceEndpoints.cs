using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Web.Auth;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class BalanceEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var balanceGroup = app.MapGroup("api/balance").WithTags("Balance");
        balanceGroup.MapGet("/show", async (
            IBalanceService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            var balance = await spaceshipResourcesService.GetForUserAsync(userId);
            if (balance == null)
                return Results.BadRequest();
            return Results.Ok(balance);
        }).RequireAuthorization();

        balanceGroup.MapGet("/add-currency", async (
            IBalanceService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            await spaceshipResourcesService.AddCurrencyAsync(userId, 100);
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);

        balanceGroup.MapGet("/add-quants", async (
            IBalanceService spaceshipResourcesService,
            ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
            {
                return Results.Unauthorized();
            }
            var userId = Guid.Parse(idUnparsed);

            await spaceshipResourcesService.AddQuantsAsync(userId, 100);
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);
    }
}