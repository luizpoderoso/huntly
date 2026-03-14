using FastEndpoints;
using Huntly.Application.Auth.Commands.Login;
using Huntly.Application.Auth.DTOs;
using MediatR;

namespace Huntly.Api.Endpoints.Auth.Login;

public class LoginEndpoint(IMediator mediator) : Endpoint<LoginRequest, TokenResponseDto>
{
    public override void Configure()
    {
        Post("login");
        Group<AuthGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var command = new LoginCommand(req.Email, req.Password);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}