namespace ApiAi.Dto;

public record ChatResponse(
    Guid Id,
    string? Description,
    DateTime CreatedAt
);