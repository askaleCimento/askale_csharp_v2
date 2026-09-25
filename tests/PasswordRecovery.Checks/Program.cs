using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AskalePortal.API.Security.Auth;
using AskalePortal.API.Security.Auth.PasswordRecovery;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}
bool Valid(ResetPasswordRequest request) => Validator.TryValidateObject(request,
    new ValidationContext(request), new List<ValidationResult>(), true);
ResetPasswordRequest Request(string password, string? confirmation = null) => new()
{
    Username = "test-user", TemporaryPassword = "0123456789ABCDEF0123456789ABCDEF",
    NewPassword = password, ConfirmPassword = confirmation ?? password
};
Check(Valid(Request("Long password 123!")), "valid password accepted");
Check(!Valid(Request("short")), "short password rejected");
Check(!Valid(Request(new string(' ', 12))), "blank password rejected");
Check(!Valid(Request("Long password 123!", "Different password")), "confirmation mismatch rejected");
Check(!Valid(Request(new string('ş', 37))), "BCrypt UTF-8 byte overflow rejected");
Check(Valid(Request(new string('ş', 36))), "72-byte boundary accepted");
var malformed = Request("Long password 123!");
malformed.TemporaryPassword = "invalid";
Check(!Valid(malformed), "malformed temporary password rejected");
var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
var hash = PasswordRecoveryService.HashSecret(secret);
Check(hash.Length == 64 && hash != secret, "only SHA-256 digest used for secret verification");
Check(hash != PasswordRecoveryService.HashSecret(Convert.ToHexString(RandomNumberGenerator.GetBytes(16))), "different secrets have different hashes");
var protector = new EphemeralDataProtectionProvider().CreateProtector(PasswordRecoveryService.ProtectionPurpose);
var encrypted = protector.Protect(secret);
Check(!encrypted.Contains(secret) && protector.Unprotect(encrypted) == secret, "queued secret encryption roundtrip");
var passwordHash = BCrypt.Net.BCrypt.HashPassword("Long password 123!", workFactor: 12);
Check(BCrypt.Net.BCrypt.Verify("Long password 123!", passwordHash), "reset hash compatible with login verifier");
var key = new string('k', 64);
var before = CredentialVersion.Create(1, passwordHash, key);
var after = CredentialVersion.Create(1, BCrypt.Net.BCrypt.HashPassword("Long password 123!", workFactor: 12), key);
Check(before != after, "new salt invalidates credential version even for same password");
Check(before != CredentialVersion.Create(2, passwordHash, key), "credential version is account-bound");
Check(before != CredentialVersion.Create(1, passwordHash, new string('x', 64)), "credential version is signing-key-bound");
using var db = new DBDataContext(new DbContextOptionsBuilder<DBDataContext>()
    .UseSqlServer("Server=localhost;Database=NotConnected;Integrated Security=true;TrustServerCertificate=true",
        sql => sql.UseCompatibilityLevel(120)).Options);
Check(db.Model.FindEntityType(typeof(AuthPasswordReset))!.FindPrimaryKey()!.Properties.Single().Name == "UserId",
    "one recovery row per account");
var now = DateTime.UtcNow;
var sql = db.AuthPasswordResets.Where(x => x.ProtectedSecret != null && x.UsedAtUtc == null &&
    x.SentAtUtc == null && x.ExpiresAtUtc > now && x.NextSendAtUtc <= now && x.SendAttempts < 3)
    .OrderBy(x => x.NextSendAtUtc).Take(10).ToQueryString();
Check(sql.Contains("AuthPasswordResets") && sql.Contains("TOP"), "mail selection translates to SQL Server query");
Check(db.Model.FindEntityType(typeof(AuthRefreshToken))!.FindProperty("CredentialVersion")!.GetMaxLength() == 64,
    "refresh token credential version mapping");
Console.WriteLine($"{passed} checks passed. No live database or SMTP was used.");
