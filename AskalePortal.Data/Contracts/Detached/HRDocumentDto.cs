#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class HRDocumentDto
{
    public int Id { get; set; }

    public int? archiveId { get; set; }

    public Guid documentId { get; set; }

    public int topId { get; set; }

    public int typeId { get; set; }

    public string typeName { get; set; }

    public string title { get; set; }

    public string fileName { get; set; }

    public int? fileSize { get; set; }

    public int createdUserId { get; set; }

    public string createdByUserName { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

    public int? createdByUserId { get; set; }

}
