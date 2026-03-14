namespace Huntly.Api.Endpoints.Auth.Register;

public record RegisterRequest(
    string FullName,
    string Username,
    string Email,
    string Password,
    string ConfirmPassword
);