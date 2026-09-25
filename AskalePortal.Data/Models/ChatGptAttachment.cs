namespace AskalePortal.Data.Models;

public partial class ChatGptAttachment
{
    public int Id { get; set; }
    public DateTime? createdDate { get; set; }
    public int? createdUserId { get; set; }
    public bool enabled { get; set; }
    public DateTime? updatedDate { get; set; }
    public int? updatedUserId { get; set; }
    public int messageId { get; set; }
    public string? fileName { get; set; }
    public string? contentType { get; set; }
    public string? filePath { get; set; }
    public string attachmentType { get; set; } = "";
}
