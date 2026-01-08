namespace ApiAi.Models;

public class ChatMessage
{
     public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ChatId { get; set; }

    public Chat? Chat { get; set; }

    public string? Sender { get; set; }   // USER / ASSISTANT

    public string? Content { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
