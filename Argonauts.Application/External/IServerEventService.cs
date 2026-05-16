using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;

namespace Argonauts.Application.External;

/// <summary>
/// 
/// </summary>
public interface IServerEventService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    Task SendToPlayerAsync(Guid playerId, string method, params object?[] args);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angleMilliradians"></param>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    Task SendToLocationAsync(int radius, int angleMilliradians, string method, params object?[] args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="arrivalTime"></param>
    /// <returns></returns>
    Task SendUserStartExploreAsync(Guid playerId, DateTime arrivalTime);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    Task SendUserExploreResultAsync(Guid playerId, ExplorationStatus result);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="success"></param>
    /// <param name="balance"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    Task SendUserExploreEndAsync(Guid playerId, bool success, Balance balance, DateTime now);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="arrivalTime"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    Task SendUserStartMoveAsync(Guid playerId, DateTime arrivalTime, Star destination);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="radius"></param>
    /// <param name="angleMilliradians"></param>
    /// <returns></returns>
    Task SendUserConfirmMoveAsync(Guid playerId, int radius, int angleMilliradians);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angleMilliradians"></param>
    /// <returns></returns>
    Task SendLocIncomeShipAsync(int radius, int angleMilliradians);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="battleId"></param>
    /// <returns></returns>
    Task SendUserBattleStartAsync(Guid playerId, Guid battleId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task SendUserBattleEndAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task SendUserBattleRoundEndAsync(Guid playerId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    Task SendUserDeathAsync(Guid playerId);
}