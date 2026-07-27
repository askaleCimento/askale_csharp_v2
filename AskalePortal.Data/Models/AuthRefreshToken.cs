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
