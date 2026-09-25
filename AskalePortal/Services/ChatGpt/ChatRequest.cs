using System.Text.Json;

namespace AskalePortal.API.Services.ChatGpt;

public sealed class ChatRequest
{
    public string? Model { get; set; }
    public List<ChatRequestMessage>? Messages { get; set; }
}
public sealed class ChatRequestMessage
{
    public string? Role { get; set; }
    public JsonElement Content { get; set; }
}

// The multipart field named "request" contains the JSON model/messages object.
public static class ChatRequestForm
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        MaxDepth = 32
    };

    public static ChatRequest Parse(string? request)
    {
        if (string.IsNullOrWhiteSpace(request) || request.Length > 16 * 1024 * 1024)
            throw Invalid();
        try
        {
            return JsonSerializer.Deserialize<ChatRequest>(request, JsonOptions) ?? throw Invalid();
        }
        catch (JsonException) { throw Invalid(); }
    }

    private static ChatServiceException Invalid() => new(400, "CHAT_INVALID_FORM",
        "request form alanı geçerli bir model ve messages JSON nesnesi içermelidir.");
}

public sealed class ChatServiceException(int status, string code, string message) : Exception(message)
{
    public int Status { get; } = status;
    public string Code { get; } = code;
}

public static class ChatRequestValidator
{
    public static (object Payload, string LastQuestion) Validate(ChatRequest request,
        OpenAiChatOptions options, int userId)
    {
        if (request.Model is null || !options.Models.TryGetValue(request.Model, out var model)
            || string.IsNullOrWhiteSpace(model))
            throw Invalid("Seçilen model yapılandırılmamış.");
        if (request.Messages is not { Count: > 0 and <= 60 } messages || messages.Count % 2 == 0)
            throw Invalid("Sohbet geçmişi geçersiz veya çok uzun. Yeni sohbet açın.");
        var output = new List<object>();
        long bytes = 0;
        int textLength = 0;
        string lastQuestion = "";
        for (var i = 0; i < messages.Count; i++)
        {
            var item = messages[i];
            var expectedRole = i % 2 == 0 ? "user" : "assistant";
            if (item is null || item.Role != expectedRole)
                throw Invalid("Mesaj sıralaması geçersiz.");
            string text;
            object content;
            if (item.Content.ValueKind == JsonValueKind.String)
            {
                text = item.Content.GetString()!;
                content = text;
            }
            else if (expectedRole == "user" && item.Content.ValueKind == JsonValueKind.Array)
            {
                if (userId is not (29 or 9 or 1280 or 1202))
                    throw new ChatServiceException(403, "CHAT_IMAGE_FORBIDDEN", "Görsel gönderme yetkiniz bulunmuyor.");
                var parts = item.Content.EnumerateArray().ToArray();
                if (parts.Length is < 2 or > 5 || !IsType(parts[0], "text") ||
                    !parts[0].TryGetProperty("text", out var textPart) || textPart.ValueKind != JsonValueKind.String)
                    throw Invalid("Görsel mesajı geçersiz.");
                text = textPart.GetString()!;
                var normalized = new List<object> { new { type = "text", text } };
                foreach (var part in parts.Skip(1))
                {
                    if (!IsType(part, "image_url") ||
                        !part.TryGetProperty("image_url", out var image) || image.ValueKind != JsonValueKind.Object ||
                        !image.TryGetProperty("url", out var urlValue) || urlValue.ValueKind != JsonValueKind.String)
                        throw Invalid("Görsel bilgisi geçersiz.");
                    var url = urlValue.GetString()!;
                    var comma = url.IndexOf(',');
                    if (comma < 0 || comma > 30) throw Invalid("Görsel biçimi geçersiz.");
                    var header = url[..comma];
                    if (header is not ("data:image/png;base64" or "data:image/jpeg;base64" or "data:image/webp;base64"))
                        throw Invalid("Yalnızca PNG, JPEG veya WebP görsel gönderilebilir.");
                    byte[] decoded;
                    try { decoded = Convert.FromBase64String(url[(comma + 1)..]); }
                    catch (FormatException) { throw Invalid("Görsel içeriği geçersiz."); }
                    bytes += decoded.Length;
                    if (bytes > 10 * 1024 * 1024)
                        throw Invalid("Sohbetteki toplam görsel boyutu 10 MB sınırını aşıyor. Yeni sohbet açın.");
                    bool valid = header.Contains("png")
                        ? decoded.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })
                        : header.Contains("jpeg")
                            ? decoded.AsSpan().StartsWith(new byte[] { 255, 216, 255 })
                            : decoded.Length >= 12 &&
                              decoded.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                              decoded.AsSpan(8, 4).SequenceEqual("WEBP"u8);
                    if (!valid) throw Invalid("Görsel içeriği dosya türüyle uyuşmuyor.");
                    normalized.Add(new { type = "image_url", image_url = new { url } });
                }
                content = normalized;
            }
            else throw Invalid("Mesaj içeriği geçersiz.");
            textLength += text.Length;
            if (string.IsNullOrWhiteSpace(text) || text.Length > 16000 || textLength > 120000)
                throw Invalid("Mesaj boş veya çok uzun. Mesajınızı kısaltın ya da yeni sohbet açın.");
            output.Add(new { role = expectedRole, content });
            lastQuestion = text;
        }
        if (bytes > 0 && model == "gpt-4")
            throw Invalid("Görsel göndermek için GPT-4o modeliyle yeni sohbet açın.");
        // Legacy GPT-4/4o chat requests use max_tokens.
        var payload = new Dictionary<string, object> {
            ["model"] = model, ["messages"] = output, ["stream"] = false,
            [model is "gpt-4" or "gpt-4o" ? "max_tokens" : "max_completion_tokens"] = 2048
        };
        return (payload, lastQuestion);
    }

    private static bool IsType(JsonElement part, string type) =>
        part.ValueKind == JsonValueKind.Object && part.TryGetProperty("type", out var value) &&
        value.ValueKind == JsonValueKind.String && value.GetString() == type;
    private static ChatServiceException Invalid(string message) => new(400, "CHAT_INVALID_REQUEST", message);
}
