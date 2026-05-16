using Argonauts.Core.Entity;
using Argonauts.Core.Repository;
using Argonauts.Infrastructure.Database.Repository.DataSources;

namespace Argonauts.Infrastructure.Database.Repository;

public class CacheMessageRepository(ICacheService cache) : IMessageRepository
{
    private readonly ICacheService _cache = cache;
    private record Chat(List<ChatMessage> Messages);

    private const int MaxMessages = 10;

    private static string BuildKey(Guid chatId) => $"chat:{chatId}:messages";

    /// <inheritdoc/>
    public async Task AddMessageAsync(Guid chatId, ChatMessage message)
    {
        var key = BuildKey(chatId);

        var chat = (await _cache.GetJsonAsync<Chat>(key)) ?? new Chat([]);

        chat.Messages.Add(message);
        if (chat.Messages.Count > MaxMessages)
        {
            chat = chat with
            {
                Messages = [.. chat.Messages.Skip(Math.Max(0, chat.Messages.Count - MaxMessages))]
            };
        }

        await _cache.SetJsonAsync(key, chat);
    }

    /// <inheritdoc/>
    public async Task<List<ChatMessage>> GetLastMessagesAsync(Guid chatId, int count = MaxMessages)
    {
        var key = BuildKey(chatId);
        var chat = (await _cache.GetJsonAsync<Chat>(key)) ?? new Chat([]);

        return [.. chat.Messages.Skip(Math.Max(0, chat.Messages.Count - count))];
    }

    /// <inheritdoc/>
    public async Task ClearAsync(Guid chatId)
    {
        var key = BuildKey(chatId);
        await _cache.RemoveAsync(key);
    }

    /// <inheritdoc/>
    public async Task<bool> HasMessagesAsync(Guid chatId)
    {
        var key = BuildKey(chatId);
        return await _cache.ExistsAsync(key);
    }
}