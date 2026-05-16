using Argonauts.Core.Entity.Quest;

namespace Argonauts.Core.Repository;

public interface IQuestRepository
{
    Task<Quest?> GetForPlayerAsync(Guid playerId);
    Task SaveAsync(Guid playerId, Quest quest);
}
