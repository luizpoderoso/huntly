namespace Huntly.Application.Shared.DTOs.Jobs;

public record NoteDto(
    Guid Id,
    string Content,
    DateTime CreatedAt
);