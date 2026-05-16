using Argonauts.Core.Entity;

namespace Argonauts.Application.Services.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ISpaceshipOnStarService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    Task<IEnumerable<SpaceshipProfile>> GetOnStarWhereUserStands(Guid ownerId);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<SpaceshipProfile>> GetTop10Async();
}
