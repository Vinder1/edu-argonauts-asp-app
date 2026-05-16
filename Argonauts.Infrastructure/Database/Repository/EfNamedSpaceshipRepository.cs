using Argonauts.Core.Entity;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
public class EfNamedSpaceshipRepository(GameDbContext context) : INamedSpaceshipRepository
{
    private readonly GameDbContext _context = context;

    /// <inheritdoc/>
    public async Task<IEnumerable<SpaceshipProfile>> GetTop10Async()
    {
        return await _context.Spaceships
            .AsNoTracking()
            .Include(s => s.Owner)
            .Include(s => s.SpaceshipCondition)
            .Where(s => s.Owner != null && s.SpaceshipCondition != null)
            .OrderByDescending(s => s.SpaceshipCondition!.Power + s.SpaceshipCondition!.MaxDurability)
            .Take(10)
            .Select(s => new SpaceshipProfile
            {
                Id = s.OwnerId,
                Name = s.Owner!.Name,
                BattlePower = s.SpaceshipCondition!.Power + s.SpaceshipCondition.MaxDurability
            })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SpaceshipProfile>> GetOnStar(Star star)
    {
        return await _context.Spaceships
            .AsNoTracking()
            .Include(s => s.Owner)
            .Include(s => s.SpaceshipCondition)
            .Where(s => s.LocatedRadius == star.Radius
                && s.LocatedAngleMilliradians == star.AngleMilliradians)
            .Select(s => new SpaceshipProfile
            {
                Id = s.OwnerId,
                Name = s.Owner!.Name,
                BattlePower = s.SpaceshipCondition!.Power + s.SpaceshipCondition.MaxDurability
            })
            .ToListAsync();
    }
}
