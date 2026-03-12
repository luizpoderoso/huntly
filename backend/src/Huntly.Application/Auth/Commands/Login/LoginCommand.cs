using Huntly.Application.Auth.DTOs;
using MediatR;

namespace Huntly.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<TokenResponseDto>;