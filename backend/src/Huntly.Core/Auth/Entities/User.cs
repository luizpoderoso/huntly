using Microsoft.AspNetCore.Identity;

namespace Huntly.Core.Auth.Entities;

public sealed class User : IdentityUser<Guid>
{
    public string FullName { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private User() { }

    public static User Create(string fullName, string email, string username)
    {
        return new User
        {
            Id = Guid.CreateVersion7(),
            FullName = fullName,
            Email = email,
            UserName = username
        };
    }

    public void ChangeFullName(string fullName)
    {
        FullName = fullName;
        UpdateTimestamp();
    }
    
    private void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}