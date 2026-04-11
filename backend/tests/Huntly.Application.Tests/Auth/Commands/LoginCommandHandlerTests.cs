using Huntly.Application.Auth.Commands.Login;
using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Exceptions;
using Huntly.Core.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Huntly.Application.Tests.Auth.Commands;

[TestFixture]
public class LoginCommandHandlerTests
{
    private UserManager<User> _userManager;
    private ITokenService _tokenService;
    private LoginCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        var store = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(store, null, null, null, null, null, null, null, null);
        _tokenService = Substitute.For<ITokenService>();

        _handler = new LoginCommandHandler(_userManager, _tokenService);
    }

    [TearDown]
    public void TearDown()
    {
        _userManager?.Dispose();
    }

    [Test]
    public async Task Handle_ValidCredentials_ReturnsTokenResponse()
    {
        var command = new LoginCommand("test@example.com", "Password123!");
        var user = User.Create("John Doe", "test@example.com", "johndoe");

        _userManager.FindByEmailAsync(command.Email).Returns(user);
        _userManager.CheckPasswordAsync(user, command.Password).Returns(true);

        var expiresAt = DateTime.UtcNow.AddHours(2);
        _tokenService.GenerateToken(user).Returns(("valid-jwt-token", expiresAt));

        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("valid-jwt-token"));
        Assert.That(result.ExpiresAt, Is.EqualTo(expiresAt));
        Assert.That(result.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public void Handle_InvalidEmail_ThrowsUnauthorizedException()
    {
        var command = new LoginCommand("wrong@example.com", "Password123!");
        
        _userManager.FindByEmailAsync(command.Email).ReturnsNull();

        var ex = Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Invalid credentials."));
    }

    [Test]
    public void Handle_InvalidPassword_ThrowsUnauthorizedException()
    {
        var command = new LoginCommand("test@example.com", "WrongPassword!");
        var user = User.Create("John Doe", "test@example.com", "johndoe");

        _userManager.FindByEmailAsync(command.Email).Returns(user);
        _userManager.CheckPasswordAsync(user, command.Password).Returns(false);

        var ex = Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Invalid credentials."));
    }
}
