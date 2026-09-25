namespace AskalePortal.Data.Models;

// One recovery request per account. TokenHash is used for verification.
// EmailMessage.emailText contains the temporary secret for the existing sender.
public sealed class AuthPasswordReset
{
    public int UserId { get; set; }
    public Guid Generation { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string PasswordSnapshot { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProtectedSecret { get; set; } // Legacy SMTP column; new requests set null.
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime WindowStartedAtUtc { get; set; }
    public int WindowCount { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? UsedAtUtc { get; set; }
    // Compatibility columns only. Delivery status is owned by the external EmailMessage sender.
    public DateTime? SentAtUtc { get; set; }
    public DateTime NextSendAtUtc { get; set; }
    public int SendAttempts { get; set; }
}
