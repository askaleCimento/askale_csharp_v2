using AskalePortal.API.Security.Auth;
using AskalePortal.API.Infrastructure.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AskalePortal.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    IOptions<JwtOptions> options) : ControllerBase
{
    private readonly JwtOptions _options = options.Value;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(
            request,
            ResolveClientIp(request.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        if (result is null)
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized,
                "AUTH_INVALID_CREDENTIALS", "Kullanıcı adı veya parola hatalı."));
        }

        SetNoStoreHeaders();
        if (!request.UseRefreshCookie)
        {
            return Ok(result);
        }

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
        return Ok(ToSignInResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RefreshRequest? request,
        CancellationToken cancellationToken)
    {
        var cookieMode = Request.Cookies.TryGetValue(
            _options.RefreshCookieName,
            out var cookieRefreshToken) &&
            !string.IsNullOrWhiteSpace(cookieRefreshToken);

        var refreshToken = cookieMode ? cookieRefreshToken : request?.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized,
                "AUTH_REFRESH_TOKEN_MISSING", "Oturum yenileme bilgisi bulunamadı."));
        }

        var effectiveRequest = new RefreshRequest
        {
            RefreshToken = refreshToken,
            Ip = request?.Ip,
            DeviceId = request?.DeviceId
        };

        var result = await authService.RefreshAsync(
            effectiveRequest,
            ResolveClientIp(request?.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        if (result is null)
        {
            if (cookieMode) DeleteRefreshCookie();
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized,
                "AUTH_INVALID_REFRESH_TOKEN", "Oturum yenilenemedi. Yeniden giriş yapın."));
        }

        SetNoStoreHeaders();
        if (!cookieMode)
        {
            return Ok(result);
        }

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
        return Ok(ToSignInResponse(result));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LogoutRequest? request,
        CancellationToken cancellationToken)
    {
        if (!TryGetIdentity(out var userId, out var sessionId))
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized,
                "AUTH_INVALID_SESSION", "Geçersiz oturum."));
        }

        var refreshToken = Request.Cookies.TryGetValue(_options.RefreshCookieName, out var cookieRefresh)
            ? cookieRefresh
            : request?.RefreshToken;

        await authService.LogoutAsync(
            userId,
            sessionId,
            new LogoutRequest
            {
                RefreshToken = refreshToken,
                AllSessions = request?.AllSessions ?? false
            },
            cancellationToken);

        DeleteRefreshCookie();
        return NoContent();
    }

    [Authorize]
    [HttpGet("session")]
    public ActionResult<SessionResponse> Session()
    {
        if (!TryGetIdentity(out var userId, out var sessionId))
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized,
                "AUTH_INVALID_SESSION", "Geçersiz oturum."));
        }

        var username = User.FindFirstValue("username") ?? string.Empty;
        var name = User.FindFirstValue("name") ?? User.Identity?.Name ?? username;
        var authorities = User.FindAll(ClaimTypes.Role).Select(x => x.Value).Distinct().ToArray();
        return Ok(new SessionResponse(userId, username, name, sessionId, authorities));
    }

    private SignInResponse ToSignInResponse(TokenResponse token) => new(
        token.AccessToken,
        token.AccessTokenExpiresAtUtc,
        token.TokenType,
        token.SessionId,
        token.User);

    private void SetRefreshCookie(string refreshToken, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(
            _options.RefreshCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = new DateTimeOffset(DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc)),
                IsEssential = true,
                Path = _options.RefreshCookiePath
            });
    }

    private void DeleteRefreshCookie()
    {
        Response.Cookies.Delete(
            _options.RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Path = _options.RefreshCookiePath
            });
    }

    private void SetNoStoreHeaders()
    {
        Response.Headers.CacheControl = "no-store, no-cache";
        Response.Headers.Pragma = "no-cache";
    }

    private string? ResolveClientIp(string? reportedIp)
    {
        var normalized = reportedIp?.Trim();
        if (!string.IsNullOrWhiteSpace(normalized) && normalized.Length <= 64) return normalized;
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private bool TryGetIdentity(out int userId, out string sessionId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirstValue("userId");
        sessionId = User.FindFirstValue("sid") ?? string.Empty;
        return int.TryParse(userIdValue, out userId) && !string.IsNullOrWhiteSpace(sessionId);
    }
}
