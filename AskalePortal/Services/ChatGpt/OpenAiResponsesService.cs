using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AskalePortal.API.Features.ChatHistory;
using Microsoft.Extensions.Options;

namespace AskalePortal.API.Services.ChatGpt;

public sealed class OpenAiResponsesService(HttpClient client, IOptions<OpenAiChatOptions> options)
{
    public async Task<JsonElement> CompleteAsync(SaveChatSession request, CancellationToken ct)
    {
        try { ChatHistoryContract.Validate(request); }
        catch (FormatException ex) { throw new ChatServiceException(400, "CHAT_INVALID_REQUEST", ex.Message); }
        if (request.Messages[^1].User != "user")
            throw new ChatServiceException(400, "CHAT_INVALID_REQUEST", "Son mesaj kullanıcıya ait olmalıdır.");
        var settings = options.Value;
        if (!settings.Models.TryGetValue(request.Model, out var model) || string.IsNullOrWhiteSpace(model))
            throw new ChatServiceException(400, "CHAT_MODEL_NOT_CONFIGURED", "Seçilen model sunucuda yapılandırılmamış.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new ChatServiceException(503, "CHAT_NOT_CONFIGURED", "Sunucuda OpenAI API anahtarı tanımlanmamış. Sistem yöneticinize başvurun.");

        var input = request.Messages.Select(m => new {
            role = m.User,
            content = m.Attachments.Count == 0 ? (object)m.Message : new object[] {
                new { type = "input_text", text = m.Message }
            }.Concat(m.Attachments.Select(f => f.MimeType.StartsWith("image/", StringComparison.Ordinal)
                ? (object)new { type = "input_image", image_url = $"data:{f.MimeType};base64,{Convert.ToBase64String(f.Bytes)}" }
                : new { type = "input_file", filename = f.Name, file_data = $"data:{f.MimeType};base64,{Convert.ToBase64String(f.Bytes)}" })).ToArray()
        }).ToArray();
        using var message = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey.Trim());
        message.Content = JsonContent.Create(new { model, input, stream = false, store = false, max_output_tokens = 4096 });
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(75));
        try
        {
            using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            await response.Content.LoadIntoBufferAsync(2 * 1024 * 1024, timeout.Token);
            var body = await response.Content.ReadAsStringAsync(timeout.Token);
            if (!response.IsSuccessStatusCode)
                throw UpstreamError((int)response.StatusCode, body);
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("status", out var status) || status.GetString() != "completed" ||
                !root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
                throw BadReply();
            var texts = new List<string>();
            foreach (var item in output.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object || !item.TryGetProperty("content", out var content) ||
                    content.ValueKind != JsonValueKind.Array) continue;
                foreach (var part in content.EnumerateArray())
                {
                    if (part.ValueKind != JsonValueKind.Object || !part.TryGetProperty("type", out var type)) continue;
                    var key = type.GetString() == "output_text" ? "text" : type.GetString() == "refusal" ? "refusal" : "";
                    if (part.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(value.GetString())) texts.Add(value.GetString()!);
                }
            }
            if (texts.Count == 0) throw BadReply();
            int tokens = 0;
            if (root.TryGetProperty("usage", out var usage) && usage.ValueKind == JsonValueKind.Object &&
                usage.TryGetProperty("output_tokens", out var tokenValue) && tokenValue.ValueKind == JsonValueKind.Number)
                tokenValue.TryGetInt32(out tokens);
            // Return only the fields consumed by Flutter, not upstream metadata.
            return JsonSerializer.SerializeToElement(new { status = "completed", output_text = string.Join("\n", texts), usage = new { output_tokens = tokens } });
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new ChatServiceException(504, "CHAT_TIMEOUT", "Yanıt zaman aşımına uğradı. Tekrar deneyebilirsiniz."); }
        catch (HttpRequestException)
        { throw new ChatServiceException(502, "CHAT_CONNECTION", "Sunucu OpenAI servisine bağlanamadı. Sunucunun internet erişimini kontrol edin."); }
        catch (JsonException) { throw BadReply(); }
        catch (InvalidOperationException) { throw BadReply(); }
    }

    private static ChatServiceException UpstreamError(int status, string body)
    {
        string? code = null;
        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.ValueKind == JsonValueKind.Object && json.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.Object && error.TryGetProperty("code", out var value) && value.ValueKind == JsonValueKind.String)
                code = value.GetString();
        }
        catch (JsonException) { }
        if (code is "insufficient_quota" or "credit_balance_exhausted")
            return new(503, "CHAT_QUOTA_EXHAUSTED", "OpenAI API kredi/kota limiti dolmuş. Sistem yöneticisi API faturalandırmasını kontrol etmelidir.");
        if (code == "model_not_found")
            return new(502, "CHAT_UPSTREAM_MODEL", "Bu API anahtarının seçilen modele erişimi yok. İzin verilen bir model seçin.");
        return status switch {
            401 or 403 => new(503, "CHAT_UPSTREAM_AUTH", "Sunucudaki OpenAI API anahtarı geçersiz veya erişim yetkisi yok."),
            429 => new(429, "CHAT_UPSTREAM_LIMIT", "OpenAI istek limiti aşıldı. Bir süre sonra tekrar deneyin."),
            400 or 404 => new(502, "CHAT_UPSTREAM_MODEL", "OpenAI modeli veya mesaj/dosya biçimini kabul etmedi. Model erişimini ve dosya türünü kontrol edin."),
            _ => new(502, "CHAT_UPSTREAM_ERROR", "OpenAI servisi şu anda yanıt veremiyor. Tekrar deneyin.")
        };
    }
    private static ChatServiceException BadReply() => new(502, "CHAT_INVALID_REPLY", "Model yanıtı boş veya tamamlanmamış. Mesajı kısaltıp tekrar deneyin.");
}
