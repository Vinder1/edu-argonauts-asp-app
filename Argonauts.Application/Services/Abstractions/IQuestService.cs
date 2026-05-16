using Argonauts.Core.Entity.Quest;

namespace Argonauts.Application.Services.Abstractions;

public interface IQuestService
{
    Task<Quest> GetOrCreateQuestAsync(Guid playerId);
    Task RegisterKillAsync(Guid playerId, int enemyLevel);
}
