using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="namedSpaceshipRepository"></param>
/// <param name="spaceshipRepository"></param>
public class SpaceshipOnStarService(
    INamedSpaceshipRepository namedSpaceshipRepository,
    ISpaceshipRepository spaceshipRepository
) : ISpaceshipOnStarService
{
    private readonly INamedSpaceshipRepository _namedSpaceshipRepository = namedSpaceshipRepository
        ?? throw new ArgumentNullException(nameof(namedSpaceshipRepository));
    private readonly ISpaceshipRepository _spaceshipRepository = spaceshipRepository
        ?? throw new ArgumentNullException(nameof(spaceshipRepository));

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SpaceshipProfile>> GetTop10Async()
    {
        return await _namedSpaceshipRepository.GetTop10Async();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SpaceshipProfile>> GetOnStarWhereUserStands(Guid ownerId)
    {
        var spaceship = await _spaceshipRepository.GetAsync(ownerId);
        if (spaceship == null)
            return [];

        var star = new Star
        {
            Radius = spaceship.LocatedRadius,
            AngleMilliradians = spaceship.LocatedAngleMilliradians
        };

        return await _namedSpaceshipRepository.GetOnStar(star);
    }
}
