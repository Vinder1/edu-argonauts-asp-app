using Argonauts.Application.Dto;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IBattleProcessService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ServiceActionResult> CreateBattleFromExploration(Guid userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="targetOwnerId"></param>
    /// <returns></returns>
    Task<ServiceActionResult> CreatePvPBattle(Guid userId, Guid targetOwnerId);
}