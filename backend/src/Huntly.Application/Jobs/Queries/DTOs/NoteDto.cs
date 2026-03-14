namespace Huntly.Application.Jobs.Queries.DTOs;

public record NoteDto(
    Guid Id,
    string Content,
    DateTime CreatedAt
);