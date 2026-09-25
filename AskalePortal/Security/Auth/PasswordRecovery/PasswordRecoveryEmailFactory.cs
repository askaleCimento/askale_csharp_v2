using System.Net;
using AskalePortal.BLL;
using AskalePortal.Data.Models;

namespace AskalePortal.API.Security.Auth.PasswordRecovery;

// Builds an entry for the existing external sender. This class never sends mail.
public sealed class PasswordRecoveryEmailFactory(IConfiguration configuration, IWebHostEnvironment environment)
{
    public EmailMessage Create(int userId, string? name, string email, string temporaryPassword,
        DateTime expiresAtUtc, DateTime queueLocalTime)
    {
        var turkeyZone = ResolveTurkeyTimeZone();
        var expiresTurkey = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc), turkeyZone);
        var requestedTurkey = expiresTurkey.AddMinutes(-15);
        var safeName = WebUtility.HtmlEncode(name ?? "");
        var safePassword = WebUtility.HtmlEncode(temporaryPassword);
        var body = new BLLActions.EmailReaderFile().BuildEmailTemplate(configuration, environment,
            "Şifre yenileme talebiniz",
            $"<table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;background:#fff7f7;border:1px solid #f1d4d4;border-radius:12px;\"><tr><td style=\"padding:22px 20px;\">" +
            $"<p style=\"margin:0 0 18px;color:#3b2828;font-size:15px;\">Sayın <strong>{safeName}</strong>,</p>" +
            "<div style=\"padding:14px 16px;background:#fff0f0;border-left:4px solid #b41c1c;border-radius:6px;margin-bottom:18px;\"><p style=\"margin:0;color:#7f1d1d;font-size:13px;line-height:20px;\"><strong>Şifre yenileme talebi alındı.</strong><br/>Bu işlemi siz başlatmadıysanız maili dikkate almayın.</p></div>" +
            "<p style=\"margin:0 0 8px;color:#604a4a;font-size:13px;\">Tek kullanımlık geçici şifreniz</p>" +
            $"<div style=\"padding:16px 12px;background:#b41c1c;border-radius:8px;text-align:center;margin-bottom:18px;\"><span style=\"font-family:Consolas,monospace;font-size:20px;letter-spacing:1px;color:#fff;font-weight:700;word-break:break-all;\">{safePassword}</span></div>" +
            "<p style=\"margin:0 0 14px;color:#3b2828;font-size:13px;line-height:20px;\"><strong>Uygulamada izlenecek yol</strong></p>" +
            "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse:collapse;width:100%;\"><tr><td width=\"28\" valign=\"top\" style=\"color:#b41c1c;font-size:16px;font-weight:700;\">1</td><td style=\"padding-bottom:8px;color:#604a4a;font-size:13px;line-height:19px;\">Giriş ekranında <strong>Şifremi unuttum</strong> seçeneğini açın.</td></tr><tr><td width=\"28\" valign=\"top\" style=\"color:#b41c1c;font-size:16px;font-weight:700;\">2</td><td style=\"padding-bottom:8px;color:#604a4a;font-size:13px;line-height:19px;\">Kullanıcı adınızı ve bu geçici şifreyi girin.</td></tr><tr><td width=\"28\" valign=\"top\" style=\"color:#b41c1c;font-size:16px;font-weight:700;\">3</td><td style=\"color:#604a4a;font-size:13px;line-height:19px;\">Yeni şifrenizi belirleyip işlemi tamamlayın.</td></tr></table>" +
            $"<div style=\"margin-top:18px;padding:13px 15px;background:#f8f4f4;border-radius:8px;color:#604a4a;font-size:12px;line-height:19px;\"><strong style=\"color:#7f1d1d;\">Süre bilgisi</strong><br/>Talep zamanı: {requestedTurkey:dd.MM.yyyy HH:mm} (Türkiye saati)<br/>Son kullanım: <strong>{expiresTurkey:dd.MM.yyyy HH:mm}</strong> (Türkiye saati)<br/>Geçici şifre talep anından itibaren 15 dakika geçerlidir. E-postanın geç ulaşması bu süreyi uzatmaz.</div>" +
            "<p style=\"margin:18px 0 0;color:#806d6d;font-size:12px;line-height:18px;\">Geçici şifre tek kullanımlıktır ve normal giriş ekranında çalışmaz. Süresi dolarsa yeni talep oluşturun. Şifrenizi kimseyle paylaşmayın.</p>" +
            "</td></tr></table>");

        return new EmailMessage
        {
            toAddress = email,
            subject = "Aşkale Portal - Şifre yenileme",
            emailText = body,
            // GetUnsend() compares plannedDate with DateTime.Now in the existing sender.
            plannedDate = queueLocalTime,
            createdDate = queueLocalTime,
            createdUserId = userId,
            isSent = false,
            enabled = true,
            mailTuru = 1,
            dosya = string.Empty,
            isSentEmailControl = false,
            meetingDetailId = null
        };
    }

    private static TimeZoneInfo ResolveTurkeyTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul"); }
        catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
    }
}
