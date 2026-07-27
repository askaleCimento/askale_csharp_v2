#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class AttachedFileDto
{
    public int Id { get; set; }

    public int moduleId { get; set; }

    public int targetId { get; set; }

    public int createdUserId { get; set; }

    public string title { get; set; }

    public string filePath { get; set; }

    public int? visitorCount { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
