using System.ComponentModel.DataAnnotations;

namespace AskalePortal.API.Security.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Token";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required, MinLength(32)]
    public string SecurityKey { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; init; } = 30;

    [Range(1, 365)]
    public int RefreshTokenDays { get; init; } = 30;
}
