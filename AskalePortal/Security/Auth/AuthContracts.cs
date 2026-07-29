using System.ComponentModel.DataAnnotations;

namespace AskalePortal.API.Security.Auth;

public sealed class LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    public string? Ip { get; init; }

    public string? DeviceId { get; init; }

    public bool UseRefreshCookie { get; init; }
}

public sealed class RefreshRequest
{
    public string? RefreshToken { get; init; }

    public string? Ip { get; init; }

    public string? DeviceId { get; init; }
}

public sealed class LogoutRequest
{
    public string? RefreshToken { get; init; }

    public bool AllSessions { get; init; }
}

public sealed record AuthUserResponse(
    int Id,
    string Username,
    string Name,
    IReadOnlyCollection<string> Authorities);

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    string TokenType,
    string SessionId,
    AuthUserResponse User);

public sealed record SessionResponse(
    int UserId,
    string Username,
    string Name,
    string SessionId,
    IReadOnlyCollection<string> Authorities);

public sealed record AuthErrorResponse(string Code, string Message);

public sealed record SignInResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string TokenType,
    string SessionId,
    AuthUserResponse User);
