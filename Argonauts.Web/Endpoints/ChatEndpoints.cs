using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Web.Auth;
using Argonauts.Web.Requests;
using Argonauts.Web.Services.Abstractions;
using Carter;

namespace Argonauts.Web.Endpoints;

/// <summary>
/// 
/// </summary>
public class ChatEndpoints : ICarterModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/chat").WithTags("Chat");

        group.MapGet("/get-10", async (IChatService chatService) =>
        {
            var messages = await chatService.GetLastMessagesAsync();
            return Results.Ok(new
            {
                messages
            });
        });
        
        group.MapPost("/send", async (
            IChatService chatService,
            IObscenityFilterService filterService,
            PostMessageRequest request) =>
        {
            var filtered = await filterService.FilterAsync(request.Text);
            var message = ChatMessage.Create(request.Author, filtered);
            await chatService.AddMessageAsync(message);
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.Any);
        
        group.MapPost("/clear", async (IChatService chatService, PostMessageRequest request) =>
        {
            var message = ChatMessage.Create(request.Author, request.Text);
            await chatService.AddMessageAsync(message);
            return Results.Ok();
        }).RequireAuthorization(AuthPolicies.AdminOnly);
    }
}