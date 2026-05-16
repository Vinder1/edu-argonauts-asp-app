using Argonauts.Core.Entity.Quest;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Mapping;
using Argonauts.Infrastructure.Database.Repository.DataSources;
using Microsoft.EntityFrameworkCore;

namespace Argonauts.Infrastructure.Database.Repository;

public class EfQuestRepository(GameDbContext context, AppToDbMapper mapper) : IQuestRepository
{
    private readonly GameDbContext _context = context;
    private readonly AppToDbMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<Quest?> GetForPlayerAsync(Guid playerId)
    {
        var quest = await _context.Quests.AsNoTracking()
            .FirstOrDefaultAsync(q => q.OwnerId == playerId);
        if (quest == null)
            return null;
        return _mapper.ToDomainQuest(quest);
    }

    public async Task SaveAsync(Guid playerId, Quest quest)
    {
        var existing = await _context.Quests
            .FirstOrDefaultAsync(q => q.OwnerId == playerId);

        if (existing != null)
        {
            existing.Level = quest.Level;
            existing.Killed = quest.Killed;
        }
        else
        {
            _context.Quests.Add(_mapper.ToDbQuest(quest, playerId));
        }

        await _context.SaveChangesAsync();
    }
}
