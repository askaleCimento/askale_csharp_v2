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
