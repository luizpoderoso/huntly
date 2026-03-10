namespace Huntly.Application.Shared.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    string Username { get; }
    bool IsAuthenticated { get; }
}