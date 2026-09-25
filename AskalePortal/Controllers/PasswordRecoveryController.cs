using AskalePortal.API.Infrastructure.Errors;
using AskalePortal.API.Security.Auth.PasswordRecovery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AskalePortal.API.Controllers;

[ApiController, AllowAnonymous, Route("api/auth"), EnableRateLimiting("password-recovery")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PasswordRecoveryController(PasswordRecoveryService service, IOptions<PasswordRecoveryOptions> options) : ControllerBase
{
    [HttpPost("forgot-password")]
    public async Task<IActionResult> Forgot([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        if (!options.Value.Enabled) return Unavailable();
        var started = System.Diagnostics.Stopwatch.StartNew();
        await service.RequestAsync(request.Username, ct);
        // Reduce normal-path timing differences without waiting for SMTP.
        var remaining = TimeSpan.FromMilliseconds(350) - started.Elapsed;
        if (remaining > TimeSpan.Zero) await Task.Delay(remaining, ct);
        return Accepted(new { message = PasswordRecoveryService.AcceptedMessage });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        if (!options.Value.Enabled) return Unavailable();
        if (!await service.ResetAsync(request, ct))
            return BadRequest(ApiErrorWriter.Create(HttpContext, 400, "AUTH_RESET_INVALID", PasswordRecoveryService.InvalidMessage));
        return Ok(new { message = "Şifreniz değiştirildi. Yeni şifrenizle giriş yapabilirsiniz." });
    }

    private ObjectResult Unavailable() => StatusCode(503, ApiErrorWriter.Create(HttpContext, 503,
        "AUTH_RECOVERY_UNAVAILABLE", "Şifre yenileme şu anda kullanılamıyor. Lütfen daha sonra tekrar deneyin."));
}
