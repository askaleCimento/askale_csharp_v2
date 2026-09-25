namespace AskalePortal.Data.Models;

public partial class ChatGptMessage
{
    public int Id { get; set; }
    public DateTime? createdDate { get; set; }
    public int? createdUserId { get; set; }
    public bool enabled { get; set; }
    public DateTime? updatedDate { get; set; }
    public int? updatedUserId { get; set; }
    public int conversationId { get; set; }
    public string role { get; set; } = "";
    public string? content { get; set; }
    public string messageType { get; set; } = "text";
    public int? usedToken { get; set; }
}
