namespace Argonauts.Core.Entity;

public class ChatMessage
{
    public Guid Id { get; init; }
    public string SenderName { get; init; } = null!;
    public string Content { get; init; } = null!;
    public DateTime Timestamp { get; init; }

    public static ChatMessage Create(string senderName, string content) =>
        new() { Id = Guid.NewGuid(), SenderName = senderName, Content = content, Timestamp = DateTime.UtcNow };
}