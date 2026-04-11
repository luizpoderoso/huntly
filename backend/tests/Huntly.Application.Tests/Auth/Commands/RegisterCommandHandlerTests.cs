using Huntly.Application.Auth.Commands.Register;
using Huntly.Application.Auth.Interfaces;
using Huntly.Application.Shared.Exceptions;
using Huntly.Core.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Huntly.Application.Tests.Auth.Commands;

[TestFixture]
public class RegisterCommandHandlerTests
{
    private UserManager<User> _userManager;
    private ITokenService _tokenService;
    private RegisterCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        var store = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(store, null, null, null, null, null, null, null, null);
        _tokenService = Substitute.For<ITokenService>();

        _handler = new RegisterCommandHandler(_userManager, _tokenService);
    }

    [TearDown]
    public void TearDown()
    {
        _userManager?.Dispose();
    }

    [Test]
    public async Task Handle_ValidCommand_RegistersUserAndReturnsToken()
    {
        var command = new RegisterCommand(FullName: "John Doe", Email: "test@example.com", Username: "johndoe", Password: "Password123!", ConfirmPassword: "Password123!");
        
        _userManager.FindByEmailAsync(command.Email).ReturnsNull();
        _userManager.FindByNameAsync(command.Username).ReturnsNull();
        _userManager.CreateAsync(Arg.Any<User>(), command.Password).Returns(IdentityResult.Success);

        var expiresAt = DateTime.UtcNow.AddHours(2);
        _tokenService.GenerateToken(Arg.Any<User>()).Returns(("new-jwt-token", expiresAt));

        var result = await _handler.Handle(command, CancellationToken.None).AsTask();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("new-jwt-token"));
        Assert.That(result.Email, Is.EqualTo("test@example.com"));
        Assert.That(result.Username, Is.EqualTo("johndoe"));
    }

    [Test]
    public void Handle_EmailAlreadyRegistered_ThrowsConflictException()
    {
        var command = new RegisterCommand(FullName: "John Doe", Email: "test@example.com", Username: "johndoe", Password: "Password123!", ConfirmPassword: "Password123!");
        var existingUser = User.Create("Other", "test@example.com", "otheruser");

        _userManager.FindByEmailAsync(command.Email).Returns(existingUser);

        var ex = Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Email is already registered."));
    }

    [Test]
    public void Handle_UsernameAlreadyRegistered_ThrowsConflictException()
    {
        var command = new RegisterCommand("John Doe", "new@example.com", "johndoe", "Password123!", "Password123!");
        var existingUser = User.Create("Other", "other@example.com", "johndoe");

        _userManager.FindByEmailAsync(command.Email).ReturnsNull();
        _userManager.FindByNameAsync(command.Username).Returns(existingUser);

        var ex = Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Username is already registered."));
    }

    [Test]
    public void Handle_CreationFails_ThrowsException()
    {
        var command = new RegisterCommand(FullName: "John Doe", Email: "test@example.com", Username: "johndoe", Password: "Password123!", ConfirmPassword: "Password123!");
        
        _userManager.FindByEmailAsync(command.Email).ReturnsNull();
        _userManager.FindByNameAsync(command.Username).ReturnsNull();

        var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak" });
        _userManager.CreateAsync(Arg.Any<User>(), command.Password).Returns(failedResult);

        var ex = Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None).AsTask());
        Assert.That(ex.Message, Is.EqualTo("Password too weak"));
    }
}
