using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility;
using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

/// <summary>
/// Repository implementation for spaceship condition operations.
/// </summary>
public class EfSpaceshipConditionRepository(GameDbContext context, AppToDbMapper mapper) : ISpaceshipConditionRepository
{
    private readonly GameDbContext _context = context;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <inheritdoc/>
    public async Task CreateIfMissing(Guid user, bool saveManually = true)
    {
        var sc = await _context.SpaceshipConditions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (sc == null)
        {
            sc = _mapper.ToDbSpaceshipCondition(DefaultSpaceshipConditionGenerator.CreateDefault(), user);
            _context.SpaceshipConditions.Add(sc);
        }
        if (saveManually)
        {
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<SpaceshipCondition?> GetForUserAsync(Guid user)
    {
        var sc = await _context.SpaceshipConditions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (sc == null)
            return null;
        return _mapper.ToDomainSpaceshipCondition(sc);
    }

    /// <inheritdoc/>
    public async Task RestoreAsync(Guid user)
    {
        var sc = await _context.SpaceshipConditions
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (sc == null)
            return;
        sc.Antimatter = sc.MaxAntimatter;
        sc.Energy = sc.MaxEnergy;
        sc.Durability = sc.MaxDurability;
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Guid user, SpaceshipCondition condition)
    {
        var sc = await _context.SpaceshipConditions
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (sc == null)
        {
            _context.SpaceshipConditions.Add(_mapper.ToDbSpaceshipCondition(condition, user));
        }
        else
        {
            sc.Durability = condition.Durability;
            sc.Antimatter = condition.Antimatter;
            sc.Energy = condition.Energy;
        }
        await _context.SaveChangesAsync();
    }
    
    /// <inheritdoc/>
    public async Task UpdateMaxAsync(Guid user, SpaceshipCondition condition)
    {
        var sc = await _context.SpaceshipConditions
            .FirstOrDefaultAsync(s => s.OwnerId == user);
        if (sc == null)
        {
            _context.SpaceshipConditions.Add(_mapper.ToDbSpaceshipCondition(condition, user));
        }
        else
        {
            sc.MaxDurability = condition.MaxDurability;
            sc.MaxAntimatter = condition.MaxAntimatter;
            sc.MaxEnergy = condition.MaxEnergy;
            sc.MaxDistance = condition.MaxDistance;
            sc.Speed = condition.Speed;
            sc.Power = condition.Power;
        }
        await _context.SaveChangesAsync();
    }
}
