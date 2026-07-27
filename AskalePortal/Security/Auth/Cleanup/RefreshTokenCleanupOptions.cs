using System.ComponentModel.DataAnnotations;

namespace AskalePortal.API.Security.Auth.Cleanup;

public sealed class RefreshTokenCleanupOptions
{
    public const string SectionName = "Auth:RefreshTokenCleanup";

    public bool Enabled { get; init; } = true;

    [Range(1, 3650)]
    public int RetentionDays { get; init; } = 90;

    [Range(1, 168)]
    public int IntervalHours { get; init; } = 24;

    [Range(100, 2000)]
    public int BatchSize { get; init; } = 1000;
}
