using Argonauts.Core.Entity;

namespace Argonauts.Application.Services.Abstractions
{
    public interface IChatService
    {
        /// <summary>
        /// Add message to storage, automatically remove the oldest
        /// </summary>
        Task AddMessageAsync(ChatMessage message);

        /// <summary>
        /// Get last (<paramref name="count"/>) messages (order based on FIFO).
        /// </summary> 
        Task<List<ChatMessage>> GetLastMessagesAsync(int count = 10);

        /// <summary>
        /// Clear history
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Check if there are any messages
        /// </summary>
        Task<bool> HasMessagesAsync();
    }
}