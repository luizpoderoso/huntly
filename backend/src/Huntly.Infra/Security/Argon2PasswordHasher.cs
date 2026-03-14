using System.Security.Cryptography;
using Huntly.Core.Auth.Entities;
using Huntly.Infra.Security.Options;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Huntly.Infra.Security;

public class Argon2PasswordHasher(IOptions<AuthOptions> options) : IPasswordHasher<User>
{
    private const int SaltSize = 16;       // 128 bits
    private const int HashSize = 32;       // 256 bits
    private const int MemorySize = 19456;  // 19MB (OWASP minimum)
    private const int Iterations = 2;
    private const int DegreeOfParallelism = 2;

    public string HashPassword(User user, string password)
    {
        // 1. Generate random salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // 2. Calculate hash
        var hash = ComputeHash(password, salt, options.Value.Pepper);

        // 3. Combine salt + hash in base64 to store
        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);

        return $"{saltBase64}:{hashBase64}";
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        // 1. Separar salt e hash
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
            return PasswordVerificationResult.Failed;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        // 2. Calcular hash da senha fornecida com o mesmo salt
        var actualHash = ComputeHash(providedPassword, salt, options.Value.Pepper);

        // 3. Comparar usando comparação de tempo constante (evita timing attacks)
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }

    private static byte[] ComputeHash(string password, byte[] salt, string pepper)
    {
        var pepperedPassword = password + pepper;
        
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(pepperedPassword))
        {
            Salt = salt,
            MemorySize = MemorySize,
            Iterations = Iterations,
            DegreeOfParallelism = DegreeOfParallelism
        };

        return argon2.GetBytes(HashSize);
    }
}