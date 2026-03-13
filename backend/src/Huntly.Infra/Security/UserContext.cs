using Huntly.Application.Shared.Interfaces;

namespace Huntly.Infra.Security;

public class UserContext : IUserContext
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
}