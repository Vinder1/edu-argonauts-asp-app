using Argonauts.Core.Entity;

namespace Argonauts.Core.Repository;

public interface IMessageRepository
{
    /// <summary>
    /// Add message to storage, automatically remove the oldest
    /// </summary>
    Task AddMessageAsync(Guid chatId, ChatMessage message);

    /// <summary>
    /// Get last (<paramref name="count"/>) messages (order based on FIFO).
    /// </summary> 
    Task<List<ChatMessage>> GetLastMessagesAsync(Guid chatId, int count = 10);

    /// <summary>
    /// Clear history
    /// </summary>
    Task ClearAsync(Guid chatId);

    /// <summary>
    /// Check if there are any messages
    /// </summary>
    Task<bool> HasMessagesAsync(Guid chatId);
}