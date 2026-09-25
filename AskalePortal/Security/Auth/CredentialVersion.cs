using System.Security.Cryptography;
using System.Text;

namespace AskalePortal.API.Security.Auth;

public static class CredentialVersion
{
    public static string Create(int userId, string passwordHash, string signingKey) =>
        Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(signingKey),
            Encoding.UTF8.GetBytes($"credential-v1:{userId}:{passwordHash}")));
}
