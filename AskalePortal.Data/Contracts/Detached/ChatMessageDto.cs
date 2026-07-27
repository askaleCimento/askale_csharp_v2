#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ChatMessageDto
{
    public int Id { get; set; }

    public DateTime? createdDate { get; set; }

    public int? createdUserId { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public string content { get; set; }

    public int? idFrom { get; set; }

    public string idTo { get; set; }

    public string timestamp { get; set; }

    public int? type { get; set; }

    public int? chatGroupId { get; set; }

    public string fileNames { get; set; }

    public bool? isSent { get; set; }

}
