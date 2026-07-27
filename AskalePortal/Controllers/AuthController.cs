using AskalePortal.API.Security.Auth;
using AskalePortal.API.Infrastructure.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AskalePortal.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromForm] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(
            request,
            ResolveClientIp(request.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return result is null
            ? Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized, "AUTH_INVALID_CREDENTIALS", "Kullanıcı adı veya parola hatalı."))
            : Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(
            request,
            ResolveClientIp(request.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return result is null
            ? Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized, "AUTH_INVALID_REFRESH_TOKEN", "Oturum yenilenemedi. Yeniden giriş yapın."))
            : Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetIdentity(out var userId, out var sessionId))
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized, "AUTH_INVALID_SESSION", "Geçersiz oturum."));
        }

        await authService.LogoutAsync(userId, sessionId, request, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("session")]
    public ActionResult<SessionResponse> Session()
    {
        if (!TryGetIdentity(out var userId, out var sessionId))
        {
            return Unauthorized(ApiErrorWriter.Create(HttpContext, StatusCodes.Status401Unauthorized, "AUTH_INVALID_SESSION", "Geçersiz oturum."));
        }

        var username = User.FindFirstValue("username") ?? string.Empty;
        var name = User.FindFirstValue("name") ?? User.Identity?.Name ?? username;
        var authorities = User.FindAll(ClaimTypes.Role).Select(x => x.Value).Distinct().ToArray();
        return Ok(new SessionResponse(userId, username, name, sessionId, authorities));
    }

    private string? ResolveClientIp(string? reportedIp)
    {
        var normalized = reportedIp?.Trim();
        if (!string.IsNullOrWhiteSpace(normalized) && normalized.Length <= 64)
        {
            return normalized;
        }

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
