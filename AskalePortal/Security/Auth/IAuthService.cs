namespace AskalePortal.API.Security.Auth;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
    Task<TokenResponse?> RefreshAsync(RefreshRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(int userId, string sessionId, LogoutRequest request, CancellationToken cancellationToken);
}
