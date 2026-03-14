namespace Huntly.Application.Shared.Interfaces;

public interface IUserContext
{
    Guid UserId { get; set; }
    string Username { get; set; }
    bool IsAuthenticated { get; set; }
}