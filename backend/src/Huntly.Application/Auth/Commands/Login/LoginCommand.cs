using Huntly.Application.Auth.DTOs;
using Mediator;

namespace Huntly.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<TokenResponseDto>;