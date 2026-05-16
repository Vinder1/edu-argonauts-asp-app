using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity;
using Argonauts.Core.Repository;

namespace Argonauts.Application.Services.Implementations;

public class ChatService(IMessageRepository messageRepository) : IChatService
{
    private readonly IMessageRepository _messageRepository = messageRepository
        ?? throw new ArgumentNullException(nameof(messageRepository));

    // this Id is actually kinda random. It doesn't have any hidden meaning
    // but don't touch it
    private static Guid CommonChatId { get; } = Guid.Parse("401511e7-fd93-42ea-9287-9cf61a246949");

    public Task AddMessageAsync(ChatMessage message)
    {
        return _messageRepository.AddMessageAsync(CommonChatId, message);
    }

    public Task ClearAsync()
    {
        return _messageRepository.ClearAsync(CommonChatId);
    }

    public Task<List<ChatMessage>> GetLastMessagesAsync(int count = 10)
    {
        return _messageRepository.GetLastMessagesAsync(CommonChatId, count);
    }

    public Task<bool> HasMessagesAsync()
    {
        return _messageRepository.HasMessagesAsync(CommonChatId);
    }
}