#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class NoteDto
{
    public int Id { get; set; }

    public int moduleId { get; set; }

    public int targetId { get; set; }

    public int createdUserId { get; set; }

    public string description { get; set; }

    public DateTime createdDate { get; set; }

    public bool enabled { get; set; }

    public DateTime? updatedDate { get; set; }

    public int? updatedUserId { get; set; }

}
