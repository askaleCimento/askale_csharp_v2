# Backend ilgili dosyaların tam kodları

## `AskalePortal/Controllers/AuthController.cs`

```csharp
using AskalePortal.API.Security.Auth;
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
    [ProducesResponseType<AuthErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromForm] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(
            request,
            ResolveClientIp(request.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return result is null
            ? Unauthorized(new AuthErrorResponse("invalid_credentials", "Kullanıcı adı veya parola hatalı."))
            : Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<AuthErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(
            request,
            ResolveClientIp(request.Ip),
            Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return result is null
            ? Unauthorized(new AuthErrorResponse("invalid_refresh_token", "Oturum yenilenemedi. Yeniden giriş yapın."))
            : Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetIdentity(out var userId, out var sessionId))
        {
            return Unauthorized(new AuthErrorResponse("invalid_session", "Geçersiz oturum."));
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
            return Unauthorized(new AuthErrorResponse("invalid_session", "Geçersiz oturum."));
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

```

## `AskalePortal/Security/Auth/AuthContracts.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace AskalePortal.API.Security.Auth;

public sealed record LoginRequest(
    [property: Required] string Username,
    [property: Required] string Password,
    string? Ip = null,
    string? DeviceId = null);

public sealed record RefreshRequest(
    [property: Required] string RefreshToken,
    string? Ip = null,
    string? DeviceId = null);

public sealed record LogoutRequest(string? RefreshToken = null, bool AllSessions = false);

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

```

## `AskalePortal/Security/Auth/AuthService.cs`

```csharp
using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AskalePortal.API.Security.Auth;

public sealed class AuthService(
    DBDataContext db,
    IOptions<JwtOptions> options,
    TimeProvider timeProvider) : IAuthService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<TokenResponse?> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var user = await db.AdminUser
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.username == request.Username && x.enabled, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.password))
        {
            return null;
        }

        return await IssueTokenPairAsync(
            user,
            NormalizeDeviceId(request.DeviceId),
            ipAddress,
            userAgent,
            null,
            cancellationToken);
    }

    public async Task<TokenResponse?> RefreshAsync(
        RefreshRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var tokenHash = HashToken(request.RefreshToken);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var stored = await db.AuthRefreshTokens
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (stored is null || !stored.IsActive(now))
        {
            return null;
        }

        var requestDeviceId = NormalizeDeviceId(request.DeviceId);
        if (!string.IsNullOrWhiteSpace(stored.DeviceId) &&
            !string.Equals(stored.DeviceId, requestDeviceId, StringComparison.Ordinal))
        {
            return null;
        }

        var user = await db.AdminUser
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == stored.UserId && x.enabled, cancellationToken);

        if (user is null)
        {
            stored.RevokedAtUtc = now;
            stored.RevokedReason = "user-disabled";
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        stored.UsedAtUtc = now;
        var response = await IssueTokenPairAsync(
            user,
            requestDeviceId ?? stored.DeviceId,
            ipAddress,
            userAgent,
            stored.SessionId,
            cancellationToken,
            saveChanges: false);

        stored.ReplacedByTokenHash = HashToken(response.RefreshToken);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return response;
    }

    public async Task<bool> LogoutAsync(
        int userId,
        string sessionId,
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        IQueryable<AuthRefreshToken> query = db.AuthRefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > now);

        if (!request.AllSessions)
        {
            query = query.Where(x => x.SessionId == sessionId);
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var hash = HashToken(request.RefreshToken);
                query = query.Where(x => x.TokenHash == hash);
            }
        }

        var tokens = await query.ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.RevokedAtUtc = now;
            token.RevokedReason = request.AllSessions ? "logout-all" : "logout";
        }

        if (tokens.Count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private async Task<TokenResponse> IssueTokenPairAsync(
        AdminUser user,
        string? deviceId,
        string? ipAddress,
        string? userAgent,
        string? existingSessionId,
        CancellationToken cancellationToken,
        bool saveChanges = true)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var accessExpiresAt = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpiresAt = now.AddDays(_options.RefreshTokenDays);
        var jwtId = Guid.NewGuid().ToString("N");
        var sessionId = existingSessionId ?? Guid.NewGuid().ToString("N");
        var roles = await BuildRolesAsync(user.roleId, cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.name ?? user.username),
            new("userId", user.Id.ToString()),
            new("username", user.username),
            new("name", user.name ?? user.username),
            new("sid", sessionId)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExpiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        var refreshToken = GenerateRefreshToken();
        db.AuthRefreshTokens.Add(new AuthRefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            JwtId = jwtId,
            SessionId = sessionId,
            DeviceId = NormalizeDeviceId(deviceId),
            CreatedAtUtc = now,
            ExpiresAtUtc = refreshExpiresAt,
            CreatedByIp = ipAddress,
            UserAgent = Truncate(userAgent, 512)
        });

        if (saveChanges)
        {
            await db.SaveChangesAsync(cancellationToken);
        }

        return new TokenResponse(
            accessToken,
            refreshToken,
            accessExpiresAt,
            refreshExpiresAt,
            "Bearer",
            sessionId,
            new AuthUserResponse(user.Id, user.username, user.name ?? user.username, roles));
    }

    private async Task<IReadOnlyCollection<string>> BuildRolesAsync(int roleId, CancellationToken cancellationToken)
    {
        var details = await db.RoleDetail
            .AsNoTracking()
            .Where(x => x.roleId == roleId && x.enabled)
            .ToListAsync(cancellationToken);

        var roles = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in details)
        {
            if (item.canAdd) roles.Add($"ROLE_{item.moduleId}_ADD");
            if (item.canDelete) roles.Add($"ROLE_{item.moduleId}_DELETE");
            if (item.canEdit) roles.Add($"ROLE_{item.moduleId}_EDIT");
            if (item.canSee) roles.Add($"ROLE_{item.moduleId}_SEE");
            if (item.canSeeLogs) roles.Add($"ROLE_{item.moduleId}_LOGS");
            if (item.canApprove) roles.Add($"ROLE_{item.moduleId}_APPROVE");
        }
        return roles;
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string? NormalizeDeviceId(string? deviceId)
    {
        var normalized = deviceId?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        return normalized.Length <= 200 ? normalized : normalized[..200];
    }

    private static string? Truncate(string? value, int maxLength) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Length <= maxLength ? value : value[..maxLength];
}

```

## `AskalePortal.Data/Models/AuthRefreshToken.cs`

```csharp
namespace AskalePortal.Data.Models;

public sealed class AuthRefreshToken
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public required string TokenHash { get; set; }
    public required string JwtId { get; set; }
    public required string SessionId { get; set; }
    public string? DeviceId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedReason { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreatedByIp { get; set; }
    public string? UserAgent { get; set; }

    public bool IsActive(DateTime utcNow) =>
        RevokedAtUtc is null && UsedAtUtc is null && ExpiresAtUtc > utcNow;
}

```

## `AskalePortal.Data/Models/DBDataContext.Auth.cs`

```csharp
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.Data.Models;

public partial class DBDataContext
{
    public DbSet<AuthRefreshToken> AuthRefreshTokens => Set<AuthRefreshToken>();

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthRefreshToken>(entity =>
        {
            entity.ToTable("AuthRefreshTokens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            entity.Property(x => x.JwtId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.SessionId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.DeviceId).HasMaxLength(200);
            entity.Property(x => x.RevokedReason).HasMaxLength(200);
            entity.Property(x => x.ReplacedByTokenHash).HasMaxLength(64);
            entity.Property(x => x.CreatedByIp).HasMaxLength(64);
            entity.Property(x => x.UserAgent).HasMaxLength(512);
            entity.HasIndex(x => x.TokenHash).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.SessionId });
            entity.HasIndex(x => x.ExpiresAtUtc);
        });
    }
}

```

## `database/migrations/001_create_auth_refresh_tokens.sql`

```sql
IF OBJECT_ID(N'dbo.AuthRefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuthRefreshTokens
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuthRefreshTokens PRIMARY KEY,
        UserId INT NOT NULL,
        TokenHash NVARCHAR(64) NOT NULL,
        JwtId NVARCHAR(64) NOT NULL,
        SessionId NVARCHAR(64) NOT NULL,
        DeviceId NVARCHAR(200) NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        ExpiresAtUtc DATETIME2 NOT NULL,
        UsedAtUtc DATETIME2 NULL,
        RevokedAtUtc DATETIME2 NULL,
        RevokedReason NVARCHAR(200) NULL,
        ReplacedByTokenHash NVARCHAR(64) NULL,
        CreatedByIp NVARCHAR(64) NULL,
        UserAgent NVARCHAR(512) NULL,
        CONSTRAINT FK_AuthRefreshTokens_AdminUser_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.AdminUser(Id)
    );

    CREATE UNIQUE INDEX UX_AuthRefreshTokens_TokenHash
        ON dbo.AuthRefreshTokens(TokenHash);
    CREATE INDEX IX_AuthRefreshTokens_UserId_SessionId
        ON dbo.AuthRefreshTokens(UserId, SessionId);
    CREATE INDEX IX_AuthRefreshTokens_ExpiresAtUtc
        ON dbo.AuthRefreshTokens(ExpiresAtUtc);
END;

```

## `docs/DEVICE_ID_AND_IP_CONTRACT.md`

```md
# Device ID ve IP sözleşmesi

## Login

`POST /api/auth/login` isteği `multipart/form-data` kabul eder.

Alanlar:

- `username`: zorunlu
- `password`: zorunlu
- `ip`: Flutter tarafından best-effort olarak gönderilir
- `deviceId`: Flutter kurulumuna ait kalıcı UUID

## Refresh

`POST /api/auth/refresh` JSON kabul eder:

```json
{
  "refreshToken": "...",
  "ip": "203.0.113.10",
  "deviceId": "8c70b687-44f0-4f31-87d4-889824b285ea"
}
```

Refresh token ilk oluşturulduğunda `DeviceId` ve `CreatedByIp` alanları kaydedilir.
Refresh sırasında token bir cihaz kimliğiyle bağlıysa aynı `deviceId` zorunludur. IP değişikliği tek başına oturumu geçersiz kılmaz; IP audit amacıyla tutulur.

```
