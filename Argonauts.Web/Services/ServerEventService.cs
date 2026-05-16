using Argonauts.Application.External;
using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Web.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Argonauts.Web.Services;

/// <summary>
/// 
/// </summary>
/// <param name="hubContext"></param>
public class ServerEventService(IHubContext<GameHub> hubContext) : IServerEventService
{
    private readonly IHubContext<GameHub> _hubContext = hubContext
        ?? throw new ArgumentNullException(nameof(hubContext));
    
    /// <inheritdoc/>
    public Task SendToPlayerAsync(Guid playerId, string method, params object?[] args)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync(method, args);
    }

    /// <inheritdoc/>
    public Task SendToLocationAsync(int radius, int angleMilliradians, string method, params object?[] args)
    {
        return _hubContext.Clients.Group($"{radius}:{angleMilliradians}").SendAsync(method, args);
    }

    /// <inheritdoc/>
    public Task SendUserStartExploreAsync(Guid playerId, DateTime arrivalTime)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_StartExplore", playerId.ToString(), arrivalTime);
    }

    /// <inheritdoc/>
    public Task SendUserExploreResultAsync(Guid playerId, ExplorationStatus result)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_ExploreResult", playerId.ToString(), result);
    }

    /// <inheritdoc/>
    public Task SendUserExploreEndAsync(Guid playerId, bool success, Balance balance, DateTime now)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_ExploreEnd", playerId.ToString(), success, balance, now);
    }

    /// <inheritdoc/>
    public Task SendUserStartMoveAsync(Guid playerId, DateTime arrivalTime, Star destination)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_StartMove", playerId.ToString(), arrivalTime, destination);
    }

    /// <inheritdoc/>
    public Task SendUserConfirmMoveAsync(Guid playerId, int radius, int angleMilliradians)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_ConfirmMove", playerId.ToString(), radius, angleMilliradians);
    }
    
    /// <inheritdoc/>
    public Task SendLocIncomeShipAsync(int radius, int angleMilliradians)
    {
        return _hubContext.Clients.Group($"{radius}:{angleMilliradians}").SendAsync("Loc_IncomeShip");
    }

    /// <inheritdoc/>
    public Task SendUserBattleStartAsync(Guid playerId, Guid battleId)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_BattleStart", playerId.ToString(), battleId.ToString());
    }

    /// <inheritdoc/>
    public Task SendUserBattleEndAsync(Guid playerId)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_BattleEnd", playerId.ToString());
    }

    /// <inheritdoc/>
    public Task SendUserBattleRoundEndAsync(Guid playerId)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_BattleRoundEnd", playerId.ToString());
    }

    /// <inheritdoc/>
    public Task SendUserDeathAsync(Guid playerId)
    {
        return _hubContext.Clients.Group($"id:{playerId}").SendAsync("User_Death", playerId.ToString());
    }
}