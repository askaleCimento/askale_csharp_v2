using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AskalePortal.API.Security.Auth.PasswordRecovery;

public sealed class ForgotPasswordRequest
{
    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;
}

public sealed class ResetPasswordRequest : IValidatableObject
{
    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required, RegularExpression("^[A-Fa-f0-9]{32}$")]
    public string TemporaryPassword { get; set; } = string.Empty;
    [Required, StringLength(72, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])[^\r\n\u2028\u2029]{6,}$",
        ErrorMessage = "Parola en az 6 karakter, bir büyük ve bir küçük harf içermelidir.")]
    public string NewPassword { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // BCrypt limits input by bytes, not UTF-16 characters.
        if (Encoding.UTF8.GetByteCount(NewPassword) > 72 || string.IsNullOrWhiteSpace(NewPassword))
            yield return new ValidationResult("Şifre en fazla 72 UTF-8 bayt olmalıdır ve boş bırakılamaz.", [nameof(NewPassword)]);
    }
}

public sealed class PasswordRecoveryOptions
{
    // Queue-only flow needs no SMTP configuration. Explicit false still disables it.
    public bool Enabled { get; set; } = true;
}
