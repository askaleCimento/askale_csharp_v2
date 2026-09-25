using System.Text.Json;
using System.IO.Compression;
namespace AskalePortal.API.Features.ChatHistory;

public sealed class SaveChatSession
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Model { get; set; } = "";
    public List<SavedChatMessage> Messages { get; set; } = [];
}
public sealed class SavedChatMessage
{
    public string User { get; set; } = "";
    public string Message { get; set; } = "";
    public int? Tokens { get; set; }
    public List<SavedChatFile> Attachments { get; set; } = [];
}
public sealed class SavedChatFile
{
    public string Name { get; set; } = "";
    public string MimeType { get; set; } = "";
    public byte[] Bytes { get; set; } = [];
}
public static class ChatHistoryContract
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { MaxDepth = 16 };
    public static readonly string[] Models = ["gpt-5.6-sol", "gpt-5.6-terra", "gpt-5.6-luna"];
    public static void Validate(SaveChatSession request)
    {
        if (request.Id == Guid.Empty || request.Version < 0 || !Models.Contains(request.Model) ||
            request.Messages is not { Count: > 0 and <= 200 })
            throw new FormatException("Sohbet bilgisi geçersiz. En fazla 200 mesaj saklanabilir.");
        long size = 0; long characters = 0;
        for (int i = 0; i < request.Messages.Count; i++)
        {
            var message = request.Messages[i];
            if (message is null || message.User != (i % 2 == 0 ? "user" : "assistant") ||
                string.IsNullOrWhiteSpace(message.Message) || message.Message.Length > 200000 ||
                message.Tokens < 0 || message.Attachments is null || message.Attachments.Count > 5 ||
                (message.User != "user" && message.Attachments.Count != 0))
                throw new FormatException("Mesaj veya dosya bilgisi geçersiz.");
            characters += message.Message.Length;
            foreach (var file in message.Attachments)
            {
                ValidateFile(file);
                size += file.Bytes.Length;
            }
        }
        if (characters > 1000000 || size > 20 * 1024 * 1024)
            throw new FormatException("Sohbet boyutu sınırı aşıldı. Yeni sohbet açın (dosyalar toplam 20 MB).");
    }
    public static void ValidateFile(SavedChatFile file)
    {
        if (file is null || string.IsNullOrWhiteSpace(file.Name) || file.Name.Length > 200 ||
            file.Name.IndexOfAny(['/', '\\', '\0']) >= 0 ||
            file.Bytes is not { Length: > 0 and <= 10 * 1024 * 1024 })
            throw new FormatException("Dosya adı veya boyutu geçersiz (dosya başına en fazla 10 MB).");
        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        string mime = ext switch {
            ".png" => "image/png", ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp", ".gif" => "image/gif",
            ".pdf" => "application/pdf", ".txt" => "text/plain", ".csv" => "text/csv",
            ".md" => "text/markdown", ".json" => "application/json",
            ".doc" => "application/msword", ".xls" => "application/vnd.ms-excel",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            _ => throw new FormatException("Bu dosya türü desteklenmiyor.")
        };
        var bytes = file.Bytes.AsSpan();
        bool valid = ext switch {
            ".png" => bytes.StartsWith(new byte[] {137,80,78,71,13,10,26,10}),
            ".jpg" or ".jpeg" => bytes.StartsWith(new byte[] {255,216,255}),
            ".webp" => bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8,4).SequenceEqual("WEBP"u8),
            ".gif" => bytes.StartsWith("GIF87a"u8) || bytes.StartsWith("GIF89a"u8),
            ".pdf" => bytes.StartsWith("%PDF-"u8),
            ".doc" or ".xls" or ".ppt" => bytes.StartsWith(new byte[] {208,207,17,224,161,177,26,225}),
            ".docx" or ".xlsx" or ".pptx" => IsOffice(file.Bytes, ext),
            _ => !bytes.Contains((byte)0) || bytes.StartsWith(new byte[]{255,254}) || bytes.StartsWith(new byte[]{254,255})
        };
        if (!valid) throw new FormatException("Dosya içeriği uzantısıyla uyuşmuyor.");
        file.MimeType = mime;
    }
    private static bool IsOffice(byte[] bytes, string extension)
    {
        try {
            using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
            if (zip.Entries.Count > 10000 || zip.Entries.Sum(x => x.Length) > 200L * 1024 * 1024) return false;
            return zip.GetEntry("[Content_Types].xml") != null && zip.GetEntry(extension switch {
                ".docx" => "word/document.xml", ".xlsx" => "xl/workbook.xml", _ => "ppt/presentation.xml"
            }) != null;
        } catch (InvalidDataException) { return false; }
    }
}
