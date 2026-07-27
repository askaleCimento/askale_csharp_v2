#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class IncomingDocumentDto
{
    public int Id { get; set; }

    public string documentNumber { get; set; }

    public bool isOutgoing { get; set; }

    public int documentOrder { get; set; }

    public int? companyId { get; set; }

    public int? userId { get; set; }

    public string userIds { get; set; }

    public DateTime incomingDate { get; set; }

    public int? typeId { get; set; }

    public bool hasAttachment { get; set; }

    public int? sourceId { get; set; }

    public string title { get; set; }

    public string notes { get; set; }

    public DateTime? documentDate { get; set; }

    public string documentSpecialNumber { get; set; }

    public string due { get; set; }

    public DateTime? dueDate { get; set; }

    public int? createdUserId { get; set; }

    public DateTime createdDate { get; set; }

    public bool isRead { get; set; }

    public DateTime? readDate { get; set; }

    public bool isCompleted { get; set; }

    public DateTime? completedDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
