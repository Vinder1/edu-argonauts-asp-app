using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Galaxy;

namespace Argonauts.Core.Repository;

/// <summary>
/// 
/// </summary>
public interface INamedSpaceshipRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="star"></param>
    /// <returns></returns>
    Task<IEnumerable<SpaceshipProfile>> GetOnStar(Star star);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<SpaceshipProfile>> GetTop10Async();
}
