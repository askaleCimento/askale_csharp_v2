using System.Data;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using AskalePortal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Security.Auth.PasswordRecovery;

public sealed class PasswordRecoveryService(DBDataContext db, PasswordRecoveryEmailFactory emails, TimeProvider clock)
{
    public const string AcceptedMessage = "Bilgileriniz kayıtlarımızla eşleşiyorsa kayıtlı e-posta adresinize geçici şifre gönderilecektir. Gelen kutunuzu ve spam klasörünü kontrol edin.";
    public const string InvalidMessage = "Geçici şifre geçersiz, kullanılmış veya süresi dolmuş. Bilgilerinizi kontrol edin ya da yeni bir geçici şifre isteyin.";

    public static string HashSecret(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    // SQL Server transaction-owned lock covers all instances, including case-insensitive usernames.
    // The recovery request and existing EmailMessage queue entry commit together.
    private async Task LockAsync(CancellationToken ct) => await db.Database.ExecuteSqlRawAsync(
        "DECLARE @r int; EXEC @r = sys.sp_getapplock @Resource=N'Askale.PasswordRecovery.v1', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000; IF @r < 0 THROW 51000, 'Recovery lock unavailable', 1;", ct);

    public async Task RequestAsync(string username, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await LockAsync(ct);
        var users = await db.AdminUser.AsNoTracking()
            .Where(x => x.username == username.Trim() && x.enabled).Take(2).ToListAsync(ct);
        if (users.Count != 1) return;
        var user = users[0];
        if (!MailAddress.TryCreate(user.email?.Trim(), out var address) ||
            !string.Equals(address.Address, user.email?.Trim(), StringComparison.OrdinalIgnoreCase)) return;

        var row = await db.AuthPasswordResets.SingleOrDefaultAsync(x => x.UserId == user.Id, ct);
        if (row is not null)
        {
            // Repeated clicks cannot invalidate a still-usable email or flood the inbox.
            // A pending legacy encrypted SMTP item has not entered EmailMessage yet.
            if (row.ProtectedSecret is null && row.UsedAtUtc == null && row.ExpiresAtUtc > now && row.FailedAttempts < 5) return;
            if (row.CreatedAtUtc > now.AddMinutes(-1)) return;
            if (row.WindowStartedAtUtc > now.AddHours(-1) && row.WindowCount >= 3) return;
        }
        else
        {
            row = new AuthPasswordReset { UserId = user.Id };
            db.AuthPasswordResets.Add(row);
        }
        if (row.WindowStartedAtUtc <= now.AddHours(-1))
        {
            row.WindowStartedAtUtc = now;
            row.WindowCount = 0;
        }
        var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        row.Generation = Guid.NewGuid();
        row.TokenHash = HashSecret(secret);
        row.PasswordSnapshot = user.password;
        row.Email = address.Address;
        // Compatibility columns from the former SMTP worker are no longer used.
        row.ProtectedSecret = null;
        row.CreatedAtUtc = now;
        row.ExpiresAtUtc = now.AddMinutes(15);
        row.WindowCount++;
        row.FailedAttempts = 0;
        row.UsedAtUtc = null;
        row.SentAtUtc = null;
        row.SendAttempts = 0;
        row.NextSendAtUtc = now;
        db.EmailMessage.Add(emails.Create(user.Id, user.name, address.Address,
            secret, row.ExpiresAtUtc, clock.GetLocalNow().DateTime));
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<bool> ResetAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockAsync(ct);
        var users = await db.AdminUser.Where(x => x.username == request.Username.Trim() && x.enabled)
            .Take(2).ToListAsync(ct);
        if (users.Count != 1) return false;
        var user = users[0];
        var row = await db.AuthPasswordResets.SingleOrDefaultAsync(x => x.UserId == user.Id, ct);
        if (row is null || row.UsedAtUtc != null || row.ExpiresAtUtc <= now || row.FailedAttempts >= 5 ||
            !string.Equals(row.PasswordSnapshot, user.password, StringComparison.Ordinal) ||
            !string.Equals(row.Email, user.email?.Trim(), StringComparison.OrdinalIgnoreCase)) return false;

        var suppliedHash = HashSecret(request.TemporaryPassword.ToUpperInvariant());
        if (!CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(row.TokenHash), Encoding.ASCII.GetBytes(suppliedHash)))
        {
            row.FailedAttempts++;
            if (row.FailedAttempts >= 5) row.ProtectedSecret = null;
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return false;
        }
        // Always generate a fresh BCrypt salt, which also changes the credential version.
        user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        row.UsedAtUtc = now;
        row.ProtectedSecret = null;
        row.PasswordSnapshot = string.Empty;
        row.TokenHash = string.Empty;
        await db.AuthRefreshTokens.Where(x => x.UserId == user.Id && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.RevokedAtUtc, now)
                .SetProperty(x => x.RevokedReason, "password-reset"), ct);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return true;
    }
}
