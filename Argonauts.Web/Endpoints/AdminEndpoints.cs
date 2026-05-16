using System.Security.Claims;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Argonauts.Web.Auth;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class AdminEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin").WithTags("AdminOnly");

        group.MapPost("/clear-valkey",
            async (ICacheService cacheService) =>
        {
            await cacheService.Flush();
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);

        group.MapGet("/me-cached", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Name = user.FindFirst("playerName")?.Value,
                Login = user.FindFirst(ClaimTypes.Name)?.Value,
                Role = user.FindFirst(ClaimTypes.Role)?.Value,
                AuthType = user.Identity?.AuthenticationType
            });
        }).RequireAuthorization(AuthPolicies.AdminOnly);
    }
}