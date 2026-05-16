using System.Security.Claims;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Utility.Content;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class QuestEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/quest").WithTags("Quest");

        group.MapGet("/", async (IQuestService questService, DataContainer dataContainer, ClaimsPrincipal user) =>
        {
            var idUnparsed = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUnparsed == null)
                return Results.Unauthorized();

            var quest = await questService.GetOrCreateQuestAsync(Guid.Parse(idUnparsed));
            dataContainer.QuestDescriptions.TryGetValue(quest.Level, out var description);
            return Results.Ok(new
            {
                quest.Level,
                quest.Killed,
                Core.Entity.Quest.Quest.KillsRequired,
                quest.IsCompleted,
                Description = description ?? "Описание квеста не найдено"
            });
        }).RequireAuthorization();
    }
}
