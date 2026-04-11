using FastEndpoints;
using Huntly.Application.Auth.Commands.Register;
using Huntly.Application.Auth.DTOs;
using Mediator;

namespace Huntly.Api.Endpoints.Auth.Register;

public class RegisterEndpoint(IMediator mediator) : Endpoint<RegisterRequest, TokenResponseDto>
{
    public override void Configure()
    {
        Post("register");
        Group<AuthGroup>();
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var command = new RegisterCommand(
            req.FullName,
            req.Username,
            req.Email,
            req.Password,
            req.ConfirmPassword);

        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}