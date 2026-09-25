using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AskalePortal.API.Services.ChatGpt;

public sealed record ChatAnswer(string Text, int? CompletionTokens);

public sealed class OpenAiChatService(HttpClient client, IOptions<OpenAiChatOptions> options)
{
    public async Task<(ChatAnswer Answer, string Question)> CompleteAsync(ChatRequest request, int userId, CancellationToken ct)
    {
        var settings = options.Value;
        var (payload, question) = ChatRequestValidator.Validate(request, settings, userId);
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new ChatServiceException(503, "CHAT_NOT_CONFIGURED",
                "Yapay zekâ servisi sunucuda yapılandırılmamış. Sistem yöneticinize başvurun.");
        using var message = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        message.Content = JsonContent.Create(payload);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(75));
        try
        {
            using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            if (!response.IsSuccessStatusCode)
            {
                var status = (int)response.StatusCode;
                throw status switch
                {
                    429 => new ChatServiceException(429, "CHAT_UPSTREAM_LIMIT",
                        "Yapay zekâ servisinin kullanım limiti doldu. Daha sonra tekrar deneyin."),
                    401 or 403 => new ChatServiceException(503, "CHAT_UPSTREAM_AUTH",
                        "Sunucunun yapay zekâ erişim ayarları kontrol edilmelidir."),
                    400 or 404 => new ChatServiceException(502, "CHAT_UPSTREAM_MODEL",
                        "Seçilen model veya istek servis tarafından kabul edilmedi. Model yapılandırmasını kontrol edin."),
                    _ => new ChatServiceException(502, "CHAT_UPSTREAM_ERROR",
                        "Yapay zekâ servisi şu anda yanıt veremiyor.")
                };
            }
            // Bound memory even if an upstream response is malformed or unexpectedly large.
            await response.Content.LoadIntoBufferAsync(2 * 1024 * 1024, timeout.Token);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(timeout.Token));
            var root = json.RootElement;
            if (!root.TryGetProperty("choices", out var choices) || choices.ValueKind != JsonValueKind.Array ||
                choices.GetArrayLength() == 0 || !choices[0].TryGetProperty("message", out var reply))
                throw BadReply();
            string? text = reply.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.String
                ? content.GetString() : null;
            if (string.IsNullOrWhiteSpace(text) && reply.TryGetProperty("refusal", out var refusal) &&
                refusal.ValueKind == JsonValueKind.String) text = refusal.GetString();
            if (string.IsNullOrWhiteSpace(text)) throw BadReply();
            int? tokens = root.TryGetProperty("usage", out var usage) && usage.ValueKind == JsonValueKind.Object &&
                usage.TryGetProperty("completion_tokens", out var tokenValue) && tokenValue.TryGetInt32(out var count)
                ? count : null;
            return (new ChatAnswer(text, tokens), question);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new ChatServiceException(504, "CHAT_TIMEOUT", "Yanıt zaman aşımına uğradı. Tekrar deneyebilirsiniz."); }
        catch (HttpRequestException)
        { throw new ChatServiceException(502, "CHAT_CONNECTION", "Yapay zekâ servisine ulaşılamadı."); }
        catch (JsonException) { throw BadReply(); }
        catch (InvalidOperationException) { throw BadReply(); }
    }
    private static ChatServiceException BadReply() =>
        new(502, "CHAT_INVALID_REPLY", "Yapay zekâ servisinden geçerli yanıt alınamadı.");
}
