namespace Huntly.Api.Endpoints.Auth.Login;

public record LoginRequest(
    string Email,
    string Password
);