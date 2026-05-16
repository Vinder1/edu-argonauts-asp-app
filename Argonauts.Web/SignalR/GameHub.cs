using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Web.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Argonauts.Web.SignalR;

/// <summary>
/// 
/// </summary>
public class GameHub(
    ISpaceshipService spaceshipService,
    IChatService chatService,
    IObscenityFilterService obscenityFilterService,
    ILogger<GameHub> logger) : Hub
{
    private readonly ISpaceshipService _spaceshipService = spaceshipService;
    private readonly IChatService _chatService = chatService;
    private readonly IObscenityFilterService _obscenityFilterService = obscenityFilterService;
    private readonly ILogger<GameHub> _logger = logger;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="messageText"></param>
    /// <returns></returns>
    public async Task SendChatMessage(string user, string messageText)
    {
        var filtered = await _obscenityFilterService.FilterAsync(messageText);
        var message = ChatMessage.Create(user, filtered);
        await Clients.All.SendAsync("ReceiveChatMessage", message);
        await _chatService.AddMessageAsync(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task MoveSpaceship(int oldRadius, int oldAngle, int newRadius, int newAngle)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{oldRadius}:{oldAngle}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{newRadius}:{newAngle}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var idUnparsed = Context.UserIdentifier;
        if (idUnparsed == null)
        {
            return;
        }
        var userId = Guid.Parse(idUnparsed);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"id:{idUnparsed}");

        var spaceship = await _spaceshipService.GetSpaceshipAsync(userId);
        if (spaceship == null)
            return;

        _logger.LogInformation("User {OwnerId} connected with spaceship in R={Radius}, A={Angle}", 
            userId, spaceship.LocatedRadius, spaceship.LocatedAngleMilliradians);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{spaceship.LocatedRadius}:{spaceship.LocatedAngleMilliradians}");
        await base.OnConnectedAsync();
    }
}