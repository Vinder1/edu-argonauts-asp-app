using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Quest;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

public class QuestService(IQuestRepository questRepository) : IQuestService
{
    private readonly IQuestRepository _questRepository = questRepository
        ?? throw new ArgumentNullException(nameof(questRepository));

    public async Task<Quest> GetOrCreateQuestAsync(Guid playerId)
    {
        var quest = await _questRepository.GetForPlayerAsync(playerId);
        if (quest != null)
            return quest;

        quest = new Quest();
        await _questRepository.SaveAsync(playerId, quest);
        return quest;
    }

    public async Task RegisterKillAsync(Guid playerId, int enemyLevel)
    {
        var quest = await _questRepository.GetForPlayerAsync(playerId);
        if (quest == null)
            return;

        if (enemyLevel != quest.Level)
            return;

        quest.Killed++;
        if (quest.Killed >= Quest.KillsRequired)
        {
            quest.Level++;
            quest.Killed = 0;
        }

        await _questRepository.SaveAsync(playerId, quest);
    }
}
