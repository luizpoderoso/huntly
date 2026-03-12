namespace Huntly.Application.Auth.DTOs;

public record TokenResponseDto(
    string Token,
    Guid UserId,
    string Username,
    string Email,
    DateTime ExpiresAt
);