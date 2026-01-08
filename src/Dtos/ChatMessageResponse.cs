namespace ApiAi.Dto;

public record ChatMessageResponse(
    Guid Id,
    string Sender,
    string Content,
    DateTime Timestamp
);