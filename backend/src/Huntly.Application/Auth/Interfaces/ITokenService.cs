using Huntly.Core.Auth.Entities;

namespace Huntly.Application.Auth.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}