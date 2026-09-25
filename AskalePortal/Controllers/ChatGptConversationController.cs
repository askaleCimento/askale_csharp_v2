using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.API.Controllers;

[ApiController]
[Authorize]
[Route("api/chatgpt/conversations")]
public sealed class ChatGptConversationController(
    DBDataContext db,
    IConfiguration configuration,
    IWebHostEnvironment env,
    ILogger<ChatGptConversationController> logger) : ControllerBase
{
    private static readonly HttpClient OpenAiClient = new();
    private static readonly HashSet<int> ImageFeatureUserIds = [29, 9, 1280, 1202];
    private static readonly HashSet<string> AllowedModels =
    [
        "gpt-5.6-sol",
        "gpt-5.6-terra",
        "gpt-5.6-luna"
    ];
    private static readonly HashSet<string> ImageExtensions = ["png", "jpg", "jpeg", "webp", "gif"];
    private static readonly HashSet<string> FileExtensions =
    [
        "pdf", "txt", "md", "json", "html", "htm", "xml",
        "doc", "docx", "rtf", "odt", "ppt", "pptx", "csv", "tsv", "xls", "xlsx",
        "c", "cpp", "h", "hpp", "cs", "java", "kt", "kts", "dart",
        "js", "jsx", "ts", "tsx", "py", "rb", "php", "go", "rs", "swift", "sql",
        "sh", "bash", "zsh", "ps1", "bat", "cmd", "yaml", "yml", "toml", "ini",
        "cfg", "conf", "properties", "gradle", "groovy", "scala", "vue", "svelte",
        "css", "scss", "sass", "less"
    ];
    private const int MaxContextMessages = 40;
    private const long MaxFileBytes = 49L * 1024L * 1024L;
    private const long MaxTotalFileBytes = 50L * 1024L * 1024L;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<object>>> List(CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        var list = await db.ChatGptConversation
            .AsNoTracking()
            .Where(x => x.createdUserId == userId && x.enabled)
            .OrderByDescending(x => x.updatedDate ?? x.createdDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new
            {
                id = x.Id,
                title = x.title,
                model = x.model,
                createdDate = x.createdDate,
                updatedDate = x.updatedDate
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        var conversation = await OwnedConversation(id, userId, ct);
        if (conversation is null) return NotFound();

        var messages = await Messages(id, ct);
        return Ok(new
        {
            conversation = ToConversationDto(conversation),
            messages = await ToMessageDtos(messages, ct)
        });
    }

    [HttpPost("message")]
    public async Task<IActionResult> Send([FromBody] ChatGptSendRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        try
        {
            var message = NormalizeText(request.Message);
            var model = NormalizeModel(request.Model);
            var conversation = await GetOrCreateConversation(request.ConversationId, message, model, userId, ct);
            var history = await ContextMessages(conversation.Id, ct);
            var answer = await CompleteAsync(BuildTextPayload(model, history, message), ct);

            await SaveMessage(conversation.Id, "user", message, "text", 0, userId, ct);
            var assistant = await SaveMessage(conversation.Id, "assistant", answer.Text, "text", answer.UsedTokens, userId, ct);
            await Touch(conversation, model, userId, ct);
            await SaveUsageLog(message, answer.UsedTokens, userId, ct);

            return Ok(new
            {
                conversationId = conversation.Id,
                messageId = assistant.Id,
                title = conversation.title,
                model = conversation.model,
                type = "text",
                answer = answer.Text,
                imageBase64 = (string?)null,
                usedToken = answer.UsedTokens
            });
        }
        catch (ChatGptApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ChatGPT message failed.");
            return StatusCode(500, new { message = "ChatGPT işlemi sırasında sunucu hatası oluştu." });
        }
    }

    [HttpPost("message-with-files")]
    [RequestSizeLimit(MaxTotalFileBytes + 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendWithFiles(
        [FromForm] int? conversationId,
        [FromForm] string message,
        [FromForm] string model,
        [FromForm] IFormFile[] files,
        CancellationToken ct)
    {
        return await SendFiles(conversationId, message, model, files, ct);
    }

    [HttpPost("message-with-images")]
    [RequestSizeLimit(MaxTotalFileBytes + 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendWithImages(
        [FromForm] int? conversationId,
        [FromForm] string message,
        [FromForm] string model,
        [FromForm] IFormFile[] images,
        CancellationToken ct)
    {
        return await SendFiles(conversationId, message, model, images, ct);
    }

    [HttpPost("generate-image")]
    public async Task<IActionResult> GenerateImage([FromBody] ChatGptGenerateImageRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();
        if (!ImageFeatureUserIds.Contains(userId))
        {
            return StatusCode(403, new { message = "Görsel üretme yetkiniz bulunmuyor." });
        }

        try
        {
            var prompt = NormalizeText(request.Prompt);
            var model = NormalizeModel(request.Model);
            var conversation = await GetOrCreateConversation(request.ConversationId, prompt, model, userId, ct);
            var response = await CompleteAsync(new
            {
                model = ResolveModel(model),
                input = prompt,
                tools = new object[] { new { type = "image_generation", model = "gpt-image-2.5-flare" } },
                tool_choice = new { type = "image_generation" },
                store = false
            }, ct);

            if (string.IsNullOrWhiteSpace(response.ImageBase64))
            {
                throw new ChatGptApiException(502, "Görsel oluşturulamadı.");
            }

            var imageBytes = Convert.FromBase64String(response.ImageBase64);
            await SaveMessage(conversation.Id, "user", prompt, "text", 0, userId, ct);
            var assistant = await SaveMessage(conversation.Id, "assistant", "Görsel oluşturuldu.", "image", response.UsedTokens, userId, ct);
            await SaveAttachment(assistant.Id, "generated.png", "image/png", "generated_image", imageBytes, conversation.Id, userId, ct);
            await Touch(conversation, model, userId, ct);
            await SaveUsageLog(prompt, response.UsedTokens, userId, ct);

            return Ok(new
            {
                conversationId = conversation.Id,
                messageId = assistant.Id,
                title = conversation.title,
                model = conversation.model,
                type = "image",
                answer = "Görsel oluşturuldu.",
                imageBase64 = response.ImageBase64,
                usedToken = response.UsedTokens
            });
        }
        catch (ChatGptApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ChatGPT image generation failed.");
            return StatusCode(500, new { message = "ChatGPT görsel işlemi sırasında sunucu hatası oluştu." });
        }
    }

    [HttpPut("{id:int}/title")]
    public async Task<IActionResult> Rename(int id, [FromBody] ChatGptRenameRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        var conversation = await OwnedConversation(id, userId, ct);
        if (conversation is null) return NotFound();
        var title = (request.Title ?? "").Trim();
        if (title.Length == 0)
        {
            return BadRequest(new { message = "Sohbet başlığı boş olamaz." });
        }

        conversation.title = TrimTitle(title);
        conversation.updatedUserId = userId;
        conversation.updatedDate = DateTime.Now;
        await db.SaveChangesAsync(ct);
        return Ok(ToConversationDto(conversation));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<int>> Delete(int id, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        var conversation = await OwnedConversation(id, userId, ct);
        if (conversation is null) return NotFound();

        var now = DateTime.Now;
        var messages = await db.ChatGptMessage
            .Where(x => x.conversationId == id && x.enabled)
            .ToListAsync(ct);
        var messageIds = messages.Select(x => x.Id).ToArray();
        var attachments = await db.ChatGptAttachment
            .Where(x => messageIds.Contains(x.messageId) && x.enabled)
            .ToListAsync(ct);

        foreach (var attachment in attachments)
        {
            DeletePhysicalFile(attachment.filePath);
            attachment.enabled = false;
            attachment.updatedUserId = userId;
            attachment.updatedDate = now;
        }
        foreach (var item in messages)
        {
            item.enabled = false;
            item.updatedUserId = userId;
            item.updatedDate = now;
        }
        conversation.enabled = false;
        conversation.updatedUserId = userId;
        conversation.updatedDate = now;
        await db.SaveChangesAsync(ct);
        return Ok(1);
    }

    private async Task<IActionResult> SendFiles(int? conversationId, string message, string model, IFormFile[] files, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId <= 0) return Unauthorized();

        try
        {
            var text = NormalizeText(message);
            var normalizedModel = NormalizeModel(model);
            var fileInputs = await ReadFiles(files, ct);
            var conversation = await GetOrCreateConversation(conversationId, text, normalizedModel, userId, ct);
            var history = await ContextMessages(conversation.Id, ct);
            var answer = await CompleteAsync(BuildFilePayload(normalizedModel, history, text, fileInputs), ct);

            var userMessage = await SaveMessage(conversation.Id, "user", text, "text", 0, userId, ct);
            foreach (var file in fileInputs)
            {
                await SaveAttachment(
                    userMessage.Id,
                    file.FileName,
                    file.ContentType,
                    file.IsImage ? "input_image" : "input_file",
                    file.Bytes,
                    conversation.Id,
                    userId,
                    ct);
            }
            var assistant = await SaveMessage(conversation.Id, "assistant", answer.Text, "text", answer.UsedTokens, userId, ct);
            await Touch(conversation, normalizedModel, userId, ct);
            await SaveUsageLog(text, answer.UsedTokens, userId, ct);

            return Ok(new
            {
                conversationId = conversation.Id,
                messageId = assistant.Id,
                title = conversation.title,
                model = conversation.model,
                type = "text",
                answer = answer.Text,
                imageBase64 = (string?)null,
                usedToken = answer.UsedTokens
            });
        }
        catch (ChatGptApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ChatGPT file message failed.");
            return StatusCode(500, new { message = "ChatGPT dosya işlemi başarısız oldu: " + ex.Message });
        }
    }

    private int CurrentUserId()
    {
        var value = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : 0;
    }

    private async Task<ChatGptConversation?> OwnedConversation(int id, int userId, CancellationToken ct) =>
        await db.ChatGptConversation.SingleOrDefaultAsync(x => x.Id == id && x.createdUserId == userId && x.enabled, ct);

    private async Task<ChatGptConversation> GetOrCreateConversation(int? conversationId, string firstMessage, string model, int userId, CancellationToken ct)
    {
        if (conversationId is not null)
        {
            return await OwnedConversation(conversationId.Value, userId, ct)
                   ?? throw new ChatGptApiException(404, "Sohbet bulunamadı veya bu kullanıcıya ait değil.");
        }

        var conversation = new ChatGptConversation
        {
            title = TrimTitle(firstMessage),
            model = model,
            createdUserId = userId,
            createdDate = DateTime.Now,
            enabled = true
        };
        db.ChatGptConversation.Add(conversation);
        await db.SaveChangesAsync(ct);
        return conversation;
    }

    private async Task<List<ChatGptMessage>> Messages(int conversationId, CancellationToken ct) =>
        await db.ChatGptMessage
            .AsNoTracking()
            .Where(x => x.conversationId == conversationId && x.enabled)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

    private async Task<List<ChatGptMessage>> ContextMessages(int conversationId, CancellationToken ct)
    {
        var messages = await Messages(conversationId, ct);
        return messages.Count <= MaxContextMessages
            ? messages
            : messages.Skip(messages.Count - MaxContextMessages).ToList();
    }

    private async Task<ChatGptMessage> SaveMessage(int conversationId, string role, string content, string messageType, int usedToken, int userId, CancellationToken ct)
    {
        var message = new ChatGptMessage
        {
            conversationId = conversationId,
            role = role,
            content = content,
            messageType = messageType,
            usedToken = usedToken,
            createdUserId = userId,
            createdDate = DateTime.Now,
            enabled = true
        };
        db.ChatGptMessage.Add(message);
        await db.SaveChangesAsync(ct);
        return message;
    }

    private async Task Touch(ChatGptConversation conversation, string model, int userId, CancellationToken ct)
    {
        conversation.model = model;
        conversation.updatedUserId = userId;
        conversation.updatedDate = DateTime.Now;
        await db.SaveChangesAsync(ct);
    }

    private async Task SaveUsageLog(string question, int usedToken, int userId, CancellationToken ct)
    {
        db.ChatGptQueries.Add(new ChatGptQueries
        {
            query = question.Length > 255 ? question[..255] : question,
            usedToken = usedToken,
            createdUserId = userId,
            createdDate = DateTime.Now,
            enabled = true
        });
        await db.SaveChangesAsync(ct);
    }

    private object BuildTextPayload(string model, IReadOnlyList<ChatGptMessage> history, string message)
    {
        var input = BuildHistoryInput(history);
        input.Add(new { role = "user", content = (object)message });
        return new { model = ResolveModel(model), input, store = false };
    }

    private object BuildFilePayload(string model, IReadOnlyList<ChatGptMessage> history, string message, IReadOnlyList<FileInput> files)
    {
        var input = BuildHistoryInput(history);
        var content = new List<object> { new { type = "input_text", text = message } };
        foreach (var file in files)
        {
            var dataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(file.Bytes)}";
            content.Add(file.IsImage
                ? new { type = "input_image", image_url = dataUrl }
                : new { type = "input_file", filename = file.FileName, file_data = dataUrl });
        }
        input.Add(new { role = "user", content = (object)content });
        return new { model = ResolveModel(model), input, store = false };
    }

    private static List<object> BuildHistoryInput(IReadOnlyList<ChatGptMessage> history)
    {
        var input = new List<object>();
        foreach (var message in history)
        {
            if (string.IsNullOrWhiteSpace(message.content)) continue;
            if (message.role is not ("user" or "assistant")) continue;
            input.Add(new { role = message.role, content = (object)message.content });
        }
        return input;
    }

    private async Task<OpenAiAnswer> CompleteAsync(object payload, CancellationToken ct)
    {
        var apiKey = configuration["OpenAI:ApiKey"]
                     ?? configuration["openai:api:key"]
                     ?? configuration["openai.api.key"]
                     ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ChatGptApiException(503, "OpenAI API anahtarı tanımlı değil.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
        request.Content = JsonContent.Create(payload);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(100));
        using var response = await OpenAiClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
        var body = await response.Content.ReadAsStringAsync(timeout.Token);
        if (!response.IsSuccessStatusCode)
        {
            throw UpstreamError((int)response.StatusCode, body);
        }

        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        var texts = new List<string>();
        string? imageBase64 = null;

        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (item.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                {
                    foreach (var part in content.EnumerateArray())
                    {
                        if (!part.TryGetProperty("type", out var typeValue)) continue;
                        var type = typeValue.GetString();
                        if (type == "output_text" && part.TryGetProperty("text", out var textValue))
                        {
                            var text = textValue.GetString();
                            if (!string.IsNullOrWhiteSpace(text)) texts.Add(text);
                        }
                        if (type == "refusal" && part.TryGetProperty("refusal", out var refusalValue))
                        {
                            var refusal = refusalValue.GetString();
                            if (!string.IsNullOrWhiteSpace(refusal)) texts.Add(refusal);
                        }
                    }
                }

                if (item.TryGetProperty("type", out var itemType) &&
                    itemType.GetString() == "image_generation_call" &&
                    item.TryGetProperty("result", out var resultValue))
                {
                    imageBase64 = resultValue.GetString();
                }
            }
        }

        var usedToken = 0;
        if (root.TryGetProperty("usage", out var usage) &&
            usage.ValueKind == JsonValueKind.Object &&
            usage.TryGetProperty("output_tokens", out var tokens))
        {
            tokens.TryGetInt32(out usedToken);
        }

        return new OpenAiAnswer(
            string.Join("\n", texts).Trim(),
            imageBase64,
            usedToken);
    }

    private static ChatGptApiException UpstreamError(int statusCode, string body)
    {
        string? code = null;
        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.Object &&
                error.TryGetProperty("code", out var value))
            {
                code = value.GetString();
            }
        }
        catch (JsonException)
        {
        }

        return code switch
        {
            "insufficient_quota" or "credit_balance_exhausted" => new ChatGptApiException(503, "OpenAI API kredi/kota limiti dolmuş."),
            "model_not_found" => new ChatGptApiException(502, "Seçilen modele API erişimi yok."),
            _ => statusCode switch
            {
                401 or 403 => new ChatGptApiException(503, "OpenAI API anahtarı geçersiz veya yetkisiz."),
                429 => new ChatGptApiException(429, "OpenAI istek limiti aşıldı. Bir süre sonra tekrar deneyin."),
                400 or 404 => new ChatGptApiException(502, "OpenAI modeli veya mesaj/dosya biçimini kabul etmedi."),
                _ => new ChatGptApiException(502, "OpenAI servisi şu anda yanıt veremiyor.")
            }
        };
    }

    private async Task<IReadOnlyList<FileInput>> ReadFiles(IFormFile[] files, CancellationToken ct)
    {
        if (files.Length == 0)
        {
            throw new ChatGptApiException(400, "En az bir dosya seçilmelidir.");
        }

        var total = files.Sum(x => x.Length);
        if (total > MaxTotalFileBytes)
        {
            throw new ChatGptApiException(413, "Toplam dosya boyutu 50 MB sınırını aşıyor.");
        }

        var result = new List<FileInput>();
        foreach (var file in files)
        {
            if (file.Length <= 0 || file.Length > MaxFileBytes)
            {
                throw new ChatGptApiException(413, "Dosya boş olamaz ve tek dosya 49 MB sınırını aşamaz.");
            }

            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
            var isImage = ImageExtensions.Contains(extension);
            if (!isImage && !FileExtensions.Contains(extension))
            {
                throw new ChatGptApiException(400, "Bu dosya türü desteklenmiyor.");
            }

            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream, ct);
            result.Add(new FileInput(fileName, ContentTypeFor(extension, file.ContentType), stream.ToArray(), isImage));
        }
        return result;
    }

    private static string ContentTypeFor(string extension, string? contentType)
    {
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            return contentType;
        }

        return extension switch
        {
            "png" => "image/png",
            "jpg" or "jpeg" => "image/jpeg",
            "webp" => "image/webp",
            "gif" => "image/gif",
            "pdf" => "application/pdf",
            "json" => "application/json",
            "csv" => "text/csv",
            "html" or "htm" => "text/html",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt" => "application/vnd.ms-powerpoint",
            "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _ => "text/plain"
        };
    }

    private async Task SaveAttachment(int messageId, string fileName, string contentType, string attachmentType, byte[] bytes, int conversationId, int userId, CancellationToken ct)
    {
        var root = AttachmentRoot();
        var directory = Path.Combine(root, "chatgpt", conversationId.ToString());
        Directory.CreateDirectory(directory);
        var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(directory, safeFileName);
        await System.IO.File.WriteAllBytesAsync(fullPath, bytes, ct);

        db.ChatGptAttachment.Add(new ChatGptAttachment
        {
            messageId = messageId,
            fileName = fileName,
            contentType = contentType,
            filePath = fullPath,
            attachmentType = attachmentType,
            createdUserId = userId,
            createdDate = DateTime.Now,
            enabled = true
        });
        await db.SaveChangesAsync(ct);
    }

    private string AttachmentRoot()
    {
        var mode = env.IsDevelopment() ? "local" : env.IsProduction() ? "server" : "test";
        return configuration[$"FilePath:{mode}"] ?? Path.Combine(env.ContentRootPath, "files");
    }

    private void DeletePhysicalFile(string? path)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ChatGPT attachment could not be deleted. Path: {Path}", path);
        }
    }

    private async Task<IReadOnlyList<object>> ToMessageDtos(List<ChatGptMessage> messages, CancellationToken ct)
    {
        var messageIds = messages.Select(x => x.Id).ToArray();
        var attachments = await db.ChatGptAttachment
            .AsNoTracking()
            .Where(x => messageIds.Contains(x.messageId) && x.enabled)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

        return messages.Select(message => new
        {
            id = message.Id,
            conversationId = message.conversationId,
            role = message.role,
            content = message.content,
            messageType = message.messageType,
            usedToken = message.usedToken,
            createdDate = message.createdDate,
            attachments = attachments
                .Where(x => x.messageId == message.Id)
                .Select(ToAttachmentDto)
                .ToList()
        }).ToList<object>();
    }

    private object ToAttachmentDto(ChatGptAttachment attachment)
    {
        var base64 = "";
        try
        {
            if (!string.IsNullOrWhiteSpace(attachment.filePath) && System.IO.File.Exists(attachment.filePath))
            {
                base64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(attachment.filePath));
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "ChatGPT attachment could not be read. Path: {Path}", attachment.filePath);
        }

        return new
        {
            id = attachment.Id,
            fileName = attachment.fileName,
            contentType = attachment.contentType,
            attachmentType = attachment.attachmentType,
            base64
        };
    }

    private static object ToConversationDto(ChatGptConversation conversation) => new
    {
        id = conversation.Id,
        title = conversation.title,
        model = conversation.model,
        createdDate = conversation.createdDate,
        updatedDate = conversation.updatedDate
    };

    private static string NormalizeText(string? text)
    {
        var value = (text ?? "").Trim();
        if (value.Length == 0) throw new ChatGptApiException(400, "Mesaj boş olamaz.");
        if (value.Length > 16000) throw new ChatGptApiException(400, "Mesaj çok uzun. Kısaltıp tekrar deneyin.");
        return value;
    }

    private static string NormalizeModel(string? model)
    {
        var value = string.IsNullOrWhiteSpace(model) ? "gpt-5.6-luna" : model.Trim();
        if (!AllowedModels.Contains(value))
        {
            throw new ChatGptApiException(400, "Seçilen model desteklenmiyor.");
        }
        return value;
    }

    private string ResolveModel(string model) => configuration[$"OpenAI:Models:{model}"] ?? model;

    private static string TrimTitle(string title)
    {
        title = title.Trim();
        return title.Length <= 250 ? title : title[..250];
    }

    private sealed record FileInput(string FileName, string ContentType, byte[] Bytes, bool IsImage);
    private sealed record OpenAiAnswer(string Text, string? ImageBase64, int UsedTokens);
}

public sealed class ChatGptSendRequest
{
    public int? ConversationId { get; set; }
    public string? Message { get; set; }
    public string? Model { get; set; }
}

public sealed class ChatGptGenerateImageRequest
{
    public int? ConversationId { get; set; }
    public string? Prompt { get; set; }
    public string? Model { get; set; }
}

public sealed class ChatGptRenameRequest
{
    public string? Title { get; set; }
}

public sealed class ChatGptApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
