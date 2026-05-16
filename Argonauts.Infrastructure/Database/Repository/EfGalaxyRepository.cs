using Argonauts.Core.Entity.Galaxy;
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
public class EfGalaxyRepository(
    GameDbContext context, 
    AppToDbMapper mapper
) : IGalaxyRepository
{
    private readonly GameDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    // private readonly IMapper _mapper = mapper;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <inheritdoc/>>
    public async Task<Galaxy?> GetAsync()
    {
        var dbGalaxy = await _context.Galaxies
            .AsNoTracking()
            .Include(g => g.Stars)
            .OrderBy(g => g.Version)
            .FirstOrDefaultAsync();
        
        if (dbGalaxy == null)
            return null;
        
        return _mapper.ToDomainGalaxy(dbGalaxy);
        // return _mapper.Map<Galaxy>(dbGalaxy);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task OverrideAsync(Galaxy entity)
    {
        var existingGalaxy = await _context.Galaxies
            .Include(g => g.Stars)
            .FirstOrDefaultAsync();
        
        if (existingGalaxy == null)
        {
            var newGalaxy = _mapper.ToDbGalaxy(entity);//_mapper.Map<DbEntity.Galaxy>(entity);
            await _context.Galaxies.AddAsync(newGalaxy);
        }
        else
        {
            _context.Galaxies.Remove(existingGalaxy);
            var updatedGalaxy = _mapper.ToDbGalaxy(entity);
            await _context.Galaxies.AddAsync(updatedGalaxy);
        }
        
        await _context.SaveChangesAsync();
    }
}