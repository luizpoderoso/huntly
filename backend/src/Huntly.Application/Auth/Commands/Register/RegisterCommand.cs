using Huntly.Application.Auth.DTOs;
using MediatR;

namespace Huntly.Application.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password
) : IRequest<TokenResponseDto>;