using Huntly.Application.Auth.DTOs;
using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Exceptions;
using Huntly.Core.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Huntly.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService)
    : IRequestHandler<RegisterCommand, TokenResponseDto>
{
    public async Task<TokenResponseDto> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null)
            throw new ConflictException("Email is already registered.");
        
        var existingUsername = await userManager.FindByNameAsync(command.Username);
        if (existingUsername is not null)
            throw new ConflictException("Username is already registered.");

        var user = User.Create(command.FullName, command.Email, command.Username);

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
            throw new Exception(result.Errors.First().Description);

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