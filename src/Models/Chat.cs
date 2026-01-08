namespace ApiAi.Models;

public class Chat
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ChatMessage> Messages { get; set; } = new();
}