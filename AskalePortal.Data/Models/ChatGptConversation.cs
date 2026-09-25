namespace AskalePortal.Data.Models;

public partial class ChatGptConversation
{
    public int Id { get; set; }
    public DateTime? createdDate { get; set; }
    public int? createdUserId { get; set; }
    public bool enabled { get; set; }
    public DateTime? updatedDate { get; set; }
    public int? updatedUserId { get; set; }
    public string? title { get; set; }
    public string? model { get; set; }
}
