using Huntly.Application.Auth.DTOs;
using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Exceptions;
using Huntly.Core.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Huntly.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, TokenResponseDto>
{
    public async Task<TokenResponseDto> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            throw new UnauthorizedException("Invalid credentials.");

        var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
        if (!isPasswordValid)
            throw new UnauthorizedException("Invalid credentials.");

        var (token, expiresAt) = tokenService.GenerateToken(user);

        return new TokenResponseDto(
            token,
            user.Id,
            user.UserName!,
            user.Email!,
            expiresAt
        );
    }
}