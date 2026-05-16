using Argonauts.Core.Repository;
using Argonauts.Core.Utility;
using Argonauts.Infrastructure.Database.Entity;
using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="mapper"></param>
public class EfSpaceshipRepository(GameDbContext context, AppToDbMapper mapper) : ISpaceshipRepository
{
    private readonly GameDbContext _dbContext = context;
    private readonly DbSet<Spaceship> _dbSet = context.Spaceships;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public async Task<Core.Entity.Player.Spaceship?> GetAsync(Guid ownerId)
    {
        var dbSpaceship = await _dbSet.AsNoTracking()
            // .Include(s => s.Owner)
            // .Include(s => s.Galaxy)
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId);
        if (dbSpaceship == null)
            return null;
        return _mapper.ToDomainSpaceship(dbSpaceship);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Core.Entity.Player.Spaceship> CreateAsync(Guid ownerId, Core.Entity.Player.Spaceship entity)
    {
        if (entity.OwnerId != ownerId)
            throw new ArgumentException("OwnerId in entity must match the provided id", nameof(ownerId));

        var dbEntity = _mapper.ToDbSpaceship(entity);
        _dbSet.Add(dbEntity);

        _dbContext.Balances.Add(new Balance { OwnerId = ownerId });
        var sc = _mapper.ToDbSpaceshipCondition(DefaultSpaceshipConditionGenerator.CreateDefault(), ownerId);
        _dbContext.SpaceshipConditions.Add(sc);

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="newRadius"></param>
    /// <param name="newAngleMilliradians"></param>
    /// <returns></returns>
    public async Task MoveAsync(Guid ownerId, int newRadius, int newAngleMilliradians)
    {
        var ship = await _dbSet.FindAsync(ownerId);
        if (ship != null)
        {
            ship.LocatedRadius = newRadius;
            ship.LocatedAngleMilliradians = newAngleMilliradians;
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Core.Entity.Player.Spaceship>> GetAllAsync()
    {
        var entities = await _dbSet.AsNoTracking()
            // .Include(s => s.Owner)
            // .Include(s => s.Galaxy)
            .ToListAsync();
        return _mapper.ToDomainSpaceships(entities);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angleMilliradians"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Core.Entity.Player.Spaceship>> GetAllOnStarAsync(int radius, int angleMilliradians)
    {
        var spaceships = await _dbSet.AsNoTracking()
            .Where(v => v.LocatedRadius == radius &&
                v.LocatedAngleMilliradians == angleMilliradians)
            .ToListAsync();

        return _mapper.ToDomainSpaceships(spaceships);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="galaxyVersion"></param>
    /// <returns></returns>
    public async Task DeleteInactiveAsync(int galaxyVersion)
    {
        var inactiveShips = await _dbSet
            .Where(s => s.GalaxyVersion < galaxyVersion)
            .ToListAsync();

        _dbSet.RemoveRange(inactiveShips);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Core.Entity.Player.Spaceship>> GetByOwnerWithVisitedStarsAsync(Guid ownerId)
    {
        var entities = await _dbSet.AsNoTracking()
            .Include(s => s.VisitedStars).ThenInclude(v => v.Star)
            .Where(s => s.OwnerId == ownerId)
            .ToListAsync();
        return _mapper.ToDomainSpaceships(entities);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task UpdateAsync(Guid ownerId, Core.Entity.Player.Spaceship entity)
    {
        if (entity.OwnerId != ownerId)
            throw new ArgumentException("OwnerId cannot be changed", nameof(ownerId));
        var dbEntity = _mapper.ToDbSpaceship(entity);
        _dbSet.Update(dbEntity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    public async Task DeleteAsync(Guid ownerId)
    {
        var spaceship = await _dbSet
            .Include(s => s.VisitedStars) // Загружаем связанные записи для каскада
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId);

        if (spaceship == null)
            return;

        _dbSet.Remove(spaceship);
        await _dbContext.SaveChangesAsync();
    }
}