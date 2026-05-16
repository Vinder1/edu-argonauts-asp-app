using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="mapper"></param>
public class EfStarVisitRepository(GameDbContext context, AppToDbMapper mapper) : IStarVisitRepository
{
    private readonly GameDbContext _context = context;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visit"></param>
    /// <returns></returns>
    public async Task Create(SpaceshipStarVisit visit)
    {
        var dbVisit = _mapper.ToDbSpaceshipVisit(visit);
        var existing = await _context.SpaceshipStarVisits.FindAsync(
            dbVisit.SpaceshipId,
            dbVisit.StarGalaxyVersion,
            dbVisit.StarRadius,
            dbVisit.StarAngleMilliradians);
        if (existing == null)
        {
            await _context.SpaceshipStarVisits.AddAsync(dbVisit);
        }
        else
        {
            existing.VisitedAt = dbVisit.VisitedAt;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceship"></param>
    /// <param name="star"></param>
    /// <returns></returns>
    public async Task<bool> ExistsActiveFor(Spaceship spaceship, Star star)
    {
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);

        return await _context.SpaceshipStarVisits
            .AsNoTracking()
            .AnyAsync(v => v.SpaceshipId == spaceship.OwnerId &&
                v.StarRadius == star.Radius &&
                v.StarAngleMilliradians == star.AngleMilliradians &&
                v.VisitedAt >= oneDayAgo);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spaceship"></param>
    /// <param name="star"></param>
    /// <returns></returns>
    public async Task<SpaceshipStarVisit?> GetActiveFor(Spaceship spaceship, Star star)
    {
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);

        var dbEntity = await _context.SpaceshipStarVisits
            .AsNoTracking()
            .Include(v => v.Spaceship)
            .Include(v => v.Star)
            .FirstOrDefaultAsync(v => v.SpaceshipId == spaceship.OwnerId &&
                v.StarRadius == star.Radius &&
                v.StarAngleMilliradians == star.AngleMilliradians &&
                v.VisitedAt >= oneDayAgo);
        if (dbEntity == null)
            return null;
        return _mapper.ToDomainSpaceshipVisit(dbEntity);
    }
}